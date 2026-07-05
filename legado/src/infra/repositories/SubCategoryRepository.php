<?php
    namespace infra\repositories;    
    use infra\MySqlRepository;
    use infra\interfaces\ISubCategoryRepository;
    use models\SubCategory;
    use models\PaginatedResults;
    use PDO;

    class SubCategoryRepository extends MySqlRepository implements ISubCategoryRepository
    {
        public function add($subCategory)
        {
            $stmt = $this->conn->prepare(
                "INSERT INTO SubCategories (title, categoryId) 
                values 
                (:title,:categoryId)"
            );
            $stmt->bindValue(':title', $subCategory->getTitle());
            $stmt->bindValue(':categoryId', $subCategory->getCategoryId());
            $stmt->execute();
            
            return $this->conn->lastInsertId();
        }

        public function remove($subCategoryId)
        {
            $stmt = $this->conn->prepare("update Products set subCategoryId = null where subCategoryId = :subCategoryId ");
            $stmt->bindParam(':subCategoryId', $subCategoryId);
            $stmt->execute();

            $stmt = $this->conn->prepare("delete from SubCategories where subCategoryId = :subCategoryId ");
            $stmt->bindParam(':subCategoryId', $subCategoryId);
            $stmt->execute();
        }

        public function update($subCategory)
        {
            $stmt = $this->conn->prepare(
                "UPDATE subcategories 
                    set 
                        title = :title, 
                        categoryId = :categoryId
                 where subCategoryId = :subCategoryId");
            
            $stmt->bindValue(':title', $subCategory->getTitle());
            $stmt->bindValue(':categoryId', $subCategory->getCategoryId());
            $stmt->bindValue(':subCategoryId', $subCategory->getSubCategoryId());
            $stmt->execute();
        }

        public function getById($id)
        {
            $stmt = $this->conn->prepare("SELECT categoryId, title, subCategoryId from SubCategories WHERE subCategoryId = :subCategoryId " );
            $stmt->bindValue(':subCategoryId', intval($id));
            $stmt->execute();
            $row = $stmt->fetch(PDO::FETCH_ASSOC);

            $subCategory = null;
            if ($row) 
                $subCategory = new SubCategory( $row['subCategoryId'],  $row['categoryId'],  $row['title']);
            
            return $subCategory;
        }

        public function total($categoryId,$search)
        {
            if (is_null($search) ||  $search == "")
                $stmt = $this->conn->prepare("SELECT count(SubCategoryId) as total FROM SubCategories where categoryId = :categoryId");
            else
            {
                $stmt = $this->conn->prepare("SELECT count(SubCategoryId) as total FROM SubCategories where categoryId = :categoryId and title like :search  " );
                $stmt->bindValue(":search", '%' . $search . '%');
            }  
            $stmt->bindValue(":categoryId", $categoryId);
            $stmt->execute();
            $total = $stmt->fetch();
            return intval($total["total"]);
        }

        public function getAll()
        {
            $subCategoriesResults = null;
            $stmt = $this->conn->prepare("SELECT SubCategoryId, Title, CategoryId FROM SubCategories order by title ");
            $stmt->execute();
            $subCategoriesResults = $stmt->fetchAll();
            
            $subCategoriesArray = array();
            foreach($subCategoriesResults as $row)
                $subCategoriesArray[] = new SubCategory($row["SubCategoryId"], $row["CategoryId"],$row["Title"]);

            return $subCategoriesArray;
        }

        public function getAllByCategory($categoryId)
        {
            $subCategoriesResults = null;
            $stmt = $this->conn->prepare("SELECT SubCategoryId, Title, CategoryId FROM SubCategories where categoryId = :categoryId order by title ");
            $stmt->bindValue(':categoryId', intval($categoryId));
            $stmt->execute();
            $subCategoriesResults = $stmt->fetchAll();
            
            $subCategoriesArray = array();
            foreach($subCategoriesResults as $row)
                $subCategoriesArray[] = new SubCategory($row["SubCategoryId"], $row["CategoryId"],$row["Title"]);

            return $subCategoriesArray;
        }

        public function getAllPaginated($categoryId, $page, $search, $pageSize)
        {
            $page = !isset($page) ? 0 : $page;
            $pageSize = !isset($pageSize) ? 5 : $pageSize;  
            $skipNumber = !is_null($page) && $page > 0 ? $pageSize * ($page - 1) : 0;
            $total = $this->total($categoryId,$search);
            $subCategoriesResults = null;

            if (is_null($search) ||  $search === "")
            {
                $stmt = $this->conn->prepare(
                    "SELECT subCategoryId,categoryId, title  
                     FROM SubCategories WHERE categoryId = :categoryId limit :pageSize OFFSET :skipNumber "
                );
            }
            else
            {
                $stmt = $this->conn->prepare( 
                    "SELECT subCategoryId, categoryId, title FROM SubCategories 
                     WHERE categoryId = :categoryId  and title like :search  order by title 
                     limit :pageSize OFFSET :skipNumber " 
                );
                $stmt->bindValue(":search", '%' . $search . '%');
            }   

            $stmt->bindValue(':pageSize', intval(trim($pageSize)), PDO::PARAM_INT);
            $stmt->bindValue(':skipNumber', intval(trim($skipNumber)), PDO::PARAM_INT);
            $stmt->bindValue(':categoryId', intval(trim($categoryId)), PDO::PARAM_INT);
            $stmt->execute();

            $subCategoriesResults = $stmt->fetchAll();
            $subCategoriesArray = array();
            foreach($subCategoriesResults as $row)
                $subCategoriesArray[] = new SubCategory($row["subCategoryId"], $row["categoryId"], $row["title"]);

            return new PaginatedResults(
                $subCategoriesArray, 
                $total, 
                count($subCategoriesArray),
                $page,
                $pageSize,
                "/admin/subcategoria?p="
            );;
        }
    }
