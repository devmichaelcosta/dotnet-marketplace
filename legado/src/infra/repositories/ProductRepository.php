<?php

namespace infra\repositories;

use infra\MySqlRepository;
use infra\interfaces\IProductRepository;
use models\Product;
use domain\entities\AttributeValue;
use models\PaginatedResults;
use infra\helpers\StatementHelper;
use PDO;

class ProductRepository extends MySqlRepository implements IProductRepository
{   
    public function totalOfProducts($search, $userId)
    {
        $stmt = null;
        $userClausule = "";
        if (is_null($search) ||  $search === "") {
            if (isset($userId) && $userId != null)
                $userClausule = " where p.userId = :userId ";

            $stmt = $this->conn->prepare("SELECT count(p.ProductId) as total FROM Products p $userClausule");
        } else {
            if (isset($userId) && $userId != null)
                $userClausule = " where p.userId = :userId ";

            $stmt = $this->conn->prepare(
                "SELECT count(p.ProductId) as total FROM Products p
                 WHERE  (p.title like :search or p.description like :search or p.Sku like :search) $userClausule "
            );
            $stmt->bindValue(":search", '%' . $search . '%');
        }
        if (isset($userId) && $userId != null)
            $stmt->bindValue(":userId", $userId);
        $stmt->execute();
        $total = $stmt->fetch();
        return intval($total["total"]);
    }

    public function getAll($page, $search, $userId, $pageSize, $site)
    {
        if (!isset($pageSize))
            $pageSize = 5;

        $skipNumber = 0;

        if (!is_null($page) && $page > 0)
            $skipNumber = $pageSize * ($page - 1);

        $stmt = null;
        $total = $this->totalOfProducts($search, $userId);
        $userClausule = "";
        if (is_null($search) ||  $search === "") {
            if (isset($userId) && $userId != null)
                $userClausule = " where p.userId = :userId ";

            $stmt = $this->conn->prepare(
                "SELECT p.ProductId, p.Title, p.Price, p.Description, p.CreatedAt, 
                        p.CreatedBy, p.Offer, p.Stock, p.Sku, Image.filename as ImageFileName,
                        p.UserId, u.Name as Seller
                        FROM Products p
                inner join users u on p.userid = u.userid
                left join (
                    select pi.ProductId, pi.filename as filename
                    from ProductsImages pi     
                )
                as Image on p.ProductId = Image.ProductId $userClausule
                group by p.productid, Image.filename 
                order by p.title
                limit :pageSize OFFSET :skipNumber "
            );
        } else {
            if (isset($userId) && $userId != null)
                $userClausule = " and p.userId = :userId ";

            $stmt = $this->conn->prepare(
                "SELECT p.ProductId, p.Title, p.Price, p.Description, p.CreatedAt, 
                            p.CreatedBy, p.Offer, p.Stock, p.Sku, Image.filename as ImageFileName,
                            p.UserId, u.Name as Seller
                            FROM Products p
                    inner join users u on p.userid = u.userid
                    left join (
                        select pi.ProductId, pi.filename as filename
                        from ProductsImages pi     
                    )
                    as Image on p.ProductId = Image.ProductId 
                    where 
                        (p.title like :search or
                        p.description like :search or
                        p.Sku like :search) $userClausule 
                    group by productid , Image.filename
                    order by p.title 
                    limit :pageSize OFFSET :skipNumber "
            );
            $stmt->bindValue(":search", '%' . $search . '%');
        }

        if (isset($userId) && $userId != null)
            $stmt->bindValue(':userId',  $userId);

        $stmt->bindValue(':pageSize', intval(trim($pageSize)), PDO::PARAM_INT);
        $stmt->bindValue(':skipNumber', intval(trim($skipNumber)), PDO::PARAM_INT);
        $stmt->execute();

        $produtosResult = $stmt->fetchAll();
        $site = !isset($site) ? false : $site;

        return new PaginatedResults(
            $produtosResult,
            $total,
            count($produtosResult),
            $page,
            $pageSize,
            $site ? "/pesquisa?p=" : "/admin/produto?p="
        );
    }

    public function add($product)
    {
        $stmt = $this->conn->prepare(
            "INSERT INTO products(Title, Description, Price, CreatedAt, CreatedBy, Offer, Stock, Sku,UserId,SubCategoryId) 
            values (:title, :desc, :price, now(), :createdBy, :offer, :stock,:sku,:userId,:subCategoryId);"
        );

        $stmt->bindValue(":title", $product->getTitle());
        $stmt->bindValue(":desc", $product->getDescription());
        $stmt->bindValue(":price", $product->getPrice());
        $stmt->bindValue(":createdBy", $product->getCreatedBy());
        $stmt->bindValue(":offer", $product->getOffer() == 'true' ? 1 : 0, PDO::PARAM_BOOL);
        $stmt->bindValue(":stock", $product->getStock());
        $stmt->bindValue(":sku", $product->getSku());
        $stmt->bindValue(":userId", $product->getUserId());
        $stmt->bindValue(":subCategoryId", $product->getSubCategoryId());


        if (!$stmt->execute()) {
            return null;
        }

        return $this->conn->lastInsertId();
    }

    public function addImages($productId, $imagesNames)
    {
        foreach ($imagesNames as $image) {
            $stmt = $this->conn->prepare(
                "INSERT INTO productsimages(ProductId, FileName) 
                    values (:ProductId, :FileName);"
            );
            $stmt->bindValue(":ProductId", $productId);
            $stmt->bindValue(":FileName", $image);
            $stmt->execute();
        }
    }

    public function removeAllImages($productId)
    {
        $stmt = $this->conn->prepare("delete from productsimages where productid = :id");
        $stmt->bindValue(':id', $productId);
        $stmt->execute();
    }

    public function remove($id)
    {
        $this->removeAllImages($id);
        $stmt = $this->conn->prepare("delete from products where productid = :id");
        $stmt->bindValue(':id', $id);
        $stmt->execute();

        return true;
    }

    public function update($product)
    {
        $stmt = $this->conn->prepare(
            "UPDATE Products set 
                title = :title, description = :description, offer = :offer, stock = :stock,
                sku = :sku, userId = :userId, price = :price, subCategoryId = :subCategoryId
                where ProductId = :productId"
        );

        $stmt->bindValue(":title", $product->getTitle());
        $stmt->bindValue(":description", $product->getDescription());
        $stmt->bindValue(":offer", $product->getOffer() == 'true' ? 1 : 0, PDO::PARAM_BOOL);
        $stmt->bindValue(":stock", $product->getStock());
        $stmt->bindValue(":sku", $product->getSku());
        $stmt->bindValue(":productId", $product->getId());
        $stmt->bindValue(":price", $product->getPrice());
        $stmt->bindValue(":userId", $product->getUserId());
        $stmt->bindValue(":subCategoryId", $product->getSubCategoryId());
        $stmt->execute();
    }

    public function getCurrentStock($productId)
    {
        $stmt = $this->conn->prepare(
            "SELECT Stock FROM Products
                WHERE ProductId = :ProductId"
        );
        $stmt->bindValue(":ProductId", $productId);
        $stmt->execute();
        $row = $stmt->fetch(PDO::FETCH_ASSOC);
        $stock = 0;
        if ($row) {
            $stock = $row['Stock'];
        }
        return $stock;
    }

    function getById($id)
    {
        $stmt = $this->conn->prepare(
            "SELECT p.ProductId, p.Title, p.Description, p.Price, 
                    p.CreatedAt, p.CreatedBy, p.Offer, p.Stock, p.Sku,
                    p.UserId  , u.name as Seller, Image.filename as ImageFileName,p.SubCategoryId, 
                    c.Title as Category, s.Title as SubCategory, c.CategoryId as CategoryId
            FROM Products p 
            inner join users u on p.userid = u.userid 
            left join subcategories s on p.SubCategoryId = s.SubCategoryId 
            left join categories c on s.categoryId = c.categoryId 
            left join (select pi.ProductId, pi.filename as filename from ProductsImages pi ) as Image on p.ProductId = Image.ProductId 
            WHERE p.ProductId = :ProductId GROUP BY p.ProductId,Image.filename"
        );
        $stmt->bindValue(":ProductId", $id);
        $stmt->execute();
        $row = $stmt->fetch(PDO::FETCH_ASSOC);
        $product = null;

        if ($row) {
            
            $product = new Product(
                $row['ProductId'],
                $row['Title'],
                $row['Price'],
                $row['Description'],
                $row['CreatedAt'],
                $row['CreatedBy'],
                $row['Offer'],
                $row['Stock'],
                $row['Sku'],
                $row['UserId'],
                $row["Seller"]
            );
            $product->setDefaultImage($row["ImageFileName"]);
            $product->setSubCategoryId($row["SubCategoryId"]);
            $product->setSubCategoryName($row["SubCategory"]);
            $product->setCategoryId($row["CategoryId"]);
            $product->setCategoryName($row["Category"]);

            $stmt = $this->conn->prepare("select * from productsimages where productid = :ProductId;");
            $stmt->bindValue(":ProductId", $id);
            $stmt->execute();
            $images = $stmt->fetchAll();
            $product->setImages($images);
        } 
        return $product;
    }

    public function decreaseStockByOrderItens($orderItens)
    {
        if (isset($orderItens)) {
            foreach ($orderItens as $item) {
                $currentQtd = $this->getCurrentStock($item->getProductId());
                $newQtd = $currentQtd - $item->getQtd();
                $stmt = $this->conn->prepare(
                    "UPDATE products
                            set Stock = :newQtd
                        WHERE productId = :productId; "
                );

                $stmt->bindValue(":newQtd", $newQtd);
                $stmt->bindValue(":productId", $item->getProductId());

                if (!$stmt->execute()) {
                    return null;
                }
            }
        }
    }

    public function getAllByUserIdSeller($userId)
    {
        $stmt = $this->conn->prepare(
            "SELECT p.ProductId, p.Title, p.Price, p.Description, p.CreatedAt, 
                        p.CreatedBy, p.Offer, p.Stock, p.Sku, Image.filename as ImageFileName,
                        p.UserId, u.Name as Seller
                        FROM Products p
                inner join users u on p.userid = u.userid
                left join (
                    select pi.ProductId, pi.filename as filename
                    from ProductsImages pi     
                )
                as Image on p.ProductId = Image.ProductId 
                where p.userId = :userId 
                group by p.productid ,Image.filename 
                order by p.title "
        );

        if (isset($userId) && $userId != null)
            $stmt->bindValue(':userId',  $userId);

        $stmt->execute();
        $produtosResult = $stmt->fetchAll();

        $products = array();
        foreach ($produtosResult as $row) {
            $prod = new Product(
                $row['ProductId'],
                $row['Title'],
                $row['Price'],
                $row['Description'],
                $row['CreatedAt'],
                $row['CreatedBy'],
                $row['Offer'],
                $row['Stock'],
                $row['Sku'],
                $row['UserId'],
                $row["Seller"]
            );
            $prod->setDefaultImage($row["ImageFileName"]);

            $products[] = $prod;
        }
        return $products;
    }

    public function getAllBySubCategoryId($subCategoryId)
    {
        $stmt = $this->conn->prepare(
            "SELECT p.ProductId, p.Title, p.Price, p.Description, p.CreatedAt, 
                        p.CreatedBy, p.Offer, p.Stock, p.Sku, Image.filename as ImageFileName,
                        p.UserId, u.Name as Seller
                        FROM Products p
                inner join users u on p.userid = u.userid
                left join (
                    select pi.ProductId, pi.filename as filename
                    from ProductsImages pi     
                )
                as Image on p.ProductId = Image.ProductId 
                where  p.subCategoryId = :subCategoryId
                group by p.productid ,Image.filename
                order by p.title "
        );

        $stmt->bindValue(':subCategoryId',  $subCategoryId);
        $stmt->execute();
        $produtosResult = $stmt->fetchAll();

        $products = array();
        foreach ($produtosResult as $row) {
            $prod = new Product(
                $row['ProductId'],
                $row['Title'],
                $row['Price'],
                $row['Description'],
                $row['CreatedAt'],
                $row['CreatedBy'],
                $row['Offer'],
                $row['Stock'],
                $row['Sku'],
                $row['UserId'],
                $row["Seller"]
            );
            $prod->setDefaultImage($row["ImageFileName"]);

            $products[] = $prod;
        }
 
        return $products;
    }

    public function getAllSimilarProducts($productId)
    {
        $stmt = $this->conn->prepare(
            "SELECT p.ProductId, p.Title, p.Price, p.Description, p.CreatedAt, 
                    p.CreatedBy, p.Offer, p.Stock, p.Sku, Image.filename as ImageFileName, p.UserId 
            FROM Products p
            left join (
                select pi.ProductId, pi.filename as filename
                from ProductsImages pi     
            )
            as Image on p.ProductId = Image.ProductId 
            where p.productId in (SELECT childproductid from similarproducts where parentproductid = :productId) 
            group by p.productid ,Image.filename
            order by p.title "
        );

        $stmt->bindValue(':productId',  $productId);

        $stmt->execute();
        $produtosResult = $stmt->fetchAll();

        $products = array();
        foreach ($produtosResult as $row) {
            $prod = new Product(
                $row['ProductId'],
                $row['Title'],
                $row['Price'],
                $row['Description'],
                $row['CreatedAt'],
                $row['CreatedBy'],
                $row['Offer'],
                $row['Stock'],
                $row['Sku'],
                $row['UserId'],
                ''
            );
            $prod->setDefaultImage($row["ImageFileName"]);

            $products[] = $prod;
        }
        return $products;
    }

    public function totalOfSimilarProducts($search, $productId)
    {
        $stmt = null;
        if (is_null($search) ||  $search === "") {
            $stmt = $this->conn->prepare("SELECT count(childproductid) as total from similarproducts where parentproductid = :productId");
        } else {
            $stmt = $this->conn->prepare(
                "SELECT count(p.ProductId) as total FROM Products p
                 WHERE (p.title like :search orp.description like :search or p.Sku like :search) and p.ProductId in (SELECT childproductid from similarproducts where parentproductid = :productId) "
            );
            $stmt->bindValue(":search", '%' . $search . '%');
        }
        $stmt->bindValue(':productId',  $productId);
        $stmt->execute();
        $total = $stmt->fetch();
        return intval($total["total"]);
    }

    public function getAllSimilarProductsPaginated($page, $search, $userId, $pageSize, $productId)
    {
        if (!isset($pageSize))
            $pageSize = 5;

        $skipNumber = 0;

        if (!is_null($page) && $page > 0)
            $skipNumber = $pageSize * ($page - 1);

        $stmt = null;
        $total = $this->totalOfSimilarProducts($search, $productId);
        $userClausule = (isset($userId) && $userId != null) ? " and p.userId = :userId " : "";

        if (is_null($search) ||  $search === "") {
            $stmt = $this->conn->prepare(
                "SELECT p.ProductId, p.Title, p.Price, p.Description, p.CreatedAt, p.CreatedBy, p.Offer, p.Stock, p.Sku, 
                        Image.filename as ImageFileName, p.UserId, u.Name as Seller
                 FROM Products p
                 inner join users u on p.UserId = u.UserId
                 LEFT JOIN (
                    select pi.ProductId, pi.filename as filename from ProductsImages pi     
                ) as Image on p.ProductId = Image.ProductId 
                where p.ProductId in (SELECT childproductid from similarproducts where parentproductid = :productId)
                      $userClausule
                GROUP BY p.productid ,Image.filename
                ORDER BY p.title
                limit :pageSize OFFSET :skipNumber "
            );
            if (isset($userId) && $userId != null)
                $stmt->bindValue(':userId', intval(trim($userId)), PDO::PARAM_INT);
        } else {
            $stmt = $this->conn->prepare(
                "SELECT p.ProductId, p.Title, p.Price, p.Description, p.CreatedAt, p.CreatedBy, p.Offer, p.Stock, p.Sku, 
                        Image.filename as ImageFileName, p.UserId, u.Name as Seller
                 FROM Products p
                 inner join users u on p.UserId = u.UserId
                 LEFT JOIN (
                        select pi.ProductId, pi.filename as filename from ProductsImages pi     
                 ) as Image on p.ProductId = Image.ProductId 
                 WHERE p.ProductId in (SELECT childproductid from similarproducts where parentproductid = :productId) AND
                       (p.title like :search or p.description like :search or p.Sku like :search) 
                       $userClausule
                 GROUP BY productid ,Image.filename
                 ORDER BY p.title 
                 LIMIT :pageSize OFFSET :skipNumber "
            );
            $stmt->bindValue(":search", '%' . $search . '%');
            if (isset($userId) && $userId != null)
                $stmt->bindValue(':userId', intval(trim($userId)), PDO::PARAM_INT);
        }

        $stmt->bindValue(":productId", intval(trim($productId)), PDO::PARAM_INT);
        $stmt->bindValue(':pageSize', intval(trim($pageSize)), PDO::PARAM_INT);
        $stmt->bindValue(':skipNumber', intval(trim($skipNumber)), PDO::PARAM_INT);

        $stmt->execute();

        $produtosResult = $stmt->fetchAll();


        return new PaginatedResults(
            $produtosResult,
            $total,
            count($produtosResult),
            $page,
            $pageSize,
            "/admin/produto?p="
        );
    }

    public function totalOfPossibleChoicesForSimilarProducts($search, $userId, $currentProductId)
    {
        $stmt = null;
        $userClausule = "";
        if (is_null($search) ||  $search === "") {
            if (isset($userId) && $userId != null)
                $userClausule = " and p.userId = :userId ";

            $stmt = $this->conn->prepare("SELECT count(p.ProductId) as total FROM Products p where p.productId <> :currentProductId $userClausule");
        } else {
            if (isset($userId) && $userId != null)
                $userClausule = " and p.userId = :userId ";

            $stmt = $this->conn->prepare(
                "SELECT count(p.ProductId) as total FROM Products p
                 WHERE  p.productId <> :currentProductId and (p.title like :search or p.description like :search or p.Sku like :search) $userClausule "
            );
            $stmt->bindValue(":search", '%' . $search . '%');
        }
        if (isset($userId) && $userId != null)
            $stmt->bindValue(":userId", $userId);

        $stmt->bindValue(":currentProductId", $currentProductId);
        $stmt->execute();
        $total = $stmt->fetch();
        return intval($total["total"]);
    }

    public function getPossibleChoicesForSimilarProducts($page, $search, $userId, $pageSize, $currentProductId)
    {
        $pageSize = (!isset($pageSize)) ? 5 : $pageSize;
        $skipNumber = (!is_null($page) && $page > 0) ?  $pageSize * ($page - 1) : 0;
        $stmt = null;
        $total = $this->totalOfPossibleChoicesForSimilarProducts($search, $userId, $currentProductId);
        $userClausule = "";

        if (is_null($search) ||  $search === "") {
            if (isset($userId) && $userId != null)
                $userClausule = " and p.userId = :userId ";

            //echo "getPossibleChoicesForSimilarProducts " . $userId;
            $stmt = $this->conn->prepare(
                "SELECT p.ProductId, p.Title, p.Price, p.Description, p.CreatedAt, 
                        p.CreatedBy, p.Offer, p.Stock, p.Sku, Image.filename as ImageFileName,
                        p.UserId, u.Name as Seller,
                        (case when sp.ParentProductId is not null then true else false end) as Associado
                        FROM Products p
                inner join users u on p.userid = u.userid
                left join (
                    select pi.ProductId, pi.filename as filename
                    from ProductsImages pi     
                )
                as Image on p.ProductId = Image.ProductId
                left join similarproducts sp on p.ProductId = sp.ChildProductId and sp.ParentProductId = :currentProductId
                WHERE p.productid  <> :currentProductId $userClausule
                group by p.productid ,Image.filename
                order by p.title
                limit :pageSize OFFSET :skipNumber "
            );
        } else {
            if (isset($userId) && $userId != null)
                $userClausule = " and p.userId = :userId ";

            $stmt = $this->conn->prepare(
                "SELECT p.ProductId, p.Title, p.Price, p.Description, p.CreatedAt, 
                        p.CreatedBy, p.Offer, p.Stock, p.Sku, Image.filename as ImageFileName,
                        p.UserId, u.Name as Seller,
                        (case when sp.ParentProductId is not null then true else false end) as Associado
                FROM Products p
                inner join users u on p.userid = u.userid
                left join (
                    select pi.ProductId, pi.filename as filename
                    from ProductsImages pi     
                )
                as Image on p.ProductId = Image.ProductId 
                left join similarproducts sp on p.ProductId = sp.ChildProductId and sp.ParentProductId = :currentProductId
                where p.productid  <> :currentProductId AND
                    (p.title like :search or
                    p.description like :search or
                    p.Sku like :search) $userClausule 
                group by productid 
                order by p.title 
                limit :pageSize OFFSET :skipNumber "
            );
            $stmt->bindValue(":search", '%' . $search . '%');
        }

        if (isset($userId) && $userId != null)
            $stmt->bindValue(':userId',  $userId);

        $stmt->bindValue(':currentProductId', intval(trim($currentProductId)), PDO::PARAM_INT);
        $stmt->bindValue(':pageSize', intval(trim($pageSize)), PDO::PARAM_INT);
        $stmt->bindValue(':skipNumber', intval(trim($skipNumber)), PDO::PARAM_INT);
        $stmt->execute();

        $produtosResult = $stmt->fetchAll();

        return new PaginatedResults(
            $produtosResult,
            $total,
            count($produtosResult),
            $page,
            $pageSize,
            "/admin/produto?p="
        );
    }

    public function getAllCurrentSimilarProductsIdsByProductId($productId)
    {
        $stmt = $this->conn->prepare("select group_concat(ChildProductId) as productsIds from similarproducts where ParentProductId = :currentProductId");
        $stmt->bindValue(':currentProductId', intval(trim($productId)), PDO::PARAM_INT);
        $stmt->execute();

        $result = $stmt->fetch();
        return $result["productsIds"];
    }

    public function removeAllSimilarProducts($productId)
    {
        $stmt = $this->conn->prepare("delete from similarproducts where ParentProductId = :productId");
        $stmt->bindValue(':productId', $productId);
        $stmt->execute();
        return true;
    }

    public function addSimilarProducts($productId, $arrayOfIdsSimilarProducts)
    {
        if (is_null($arrayOfIdsSimilarProducts)) {
            return;
        } else {
            foreach ($arrayOfIdsSimilarProducts as $childProductId) {
                $stmt = $this->conn->prepare("INSERT INTO similarproducts(ParentProductId,ChildProductId) VALUES (:ParentProductId,:ChildProductId)");
                $stmt->bindValue(':ParentProductId', $productId);
                $stmt->bindValue(':ChildProductId', $childProductId);
                $stmt->execute();
            }
        }
    }

    public function deleteSimilarProduct($parentProductId, $childProductId)
    {
        $stmt = $this->conn->prepare("delete from similarproducts where parentProductId = :parentProductId and childProductId = :childProductId ");
        $stmt->bindValue(':parentProductId', $parentProductId);
        $stmt->bindValue(':childProductId', $childProductId);
        $stmt->execute();
    }

    public function like($productId, $userId)
    {
        $stmt = $this->conn->prepare(
            "INSERT INTO productslikeds(ProductId, UserId) select :ProductId, :UserId
             WHERE (
                 select COUNT(productlikedid) from productslikeds 
                 where  ProductId = :ProductId and UserId = :UserId) = 0;"
        );
        $stmt->bindValue(':ProductId', $productId);
        $stmt->bindValue(':UserId', $userId);
        $affectedsRows = $stmt->execute();
    }

    public function dislike($productId, $userId)
    {
        $stmt = $this->conn->prepare(
            "DELETE FROM productslikeds WHERE ProductId = :ProductId and UserId = :UserId"
        );
        $stmt->bindValue(':ProductId', $productId);
        $stmt->bindValue(':UserId', $userId);
        $stmt->execute();
    }

    public function isLiked($productId, $userId): bool
    {
        $stmt = $this->conn->prepare(
            "SELECT count(productLikedId) as total from productslikeds where  ProductId = :ProductId and UserId = :UserId"
        );
        $stmt->bindValue(":UserId", $userId);
        $stmt->bindValue(':ProductId',  $productId);
        $stmt->execute();

        $total = $stmt->fetch();
        $isLiked = (intval($total["total"]) == 0 ? false : true);
        return $isLiked;
    }

    public function addAttributeValue($attributeValue)
    {
        $stmt = $this->conn->prepare(
            "INSERT INTO attributevalues(ProductId, AttributeId, Value) 
            VALUES (:ProductId, :AttributeId, :Value);");
        $stmt->bindValue(":AttributeId", $attributeValue->getAttributeId());
        $stmt->bindValue(':ProductId',  $attributeValue->getProductId());
        $stmt->bindValue(':Value',  $attributeValue->getValue());
        $stmt->execute();
    }

    public function getAllAttributesValues($productId)
    {
        $stmt = $this->conn->prepare(
            "SELECT a.Name as Attribute, av.Value, av.ProductId, av.AttributeId FROM attributevalues av
             inner join attributes a on av.AttributeId = a.AttributeId
             where av.ProductId = :productId;"
        );

        $stmt->bindValue(':productId',  $productId);
        $stmt->execute();
        $attributesValuesRows = $stmt->fetchAll();

        $attributesValues = array();
        foreach ($attributesValuesRows as $attributeValueRow) {
            $attributeValue = new AttributeValue(
                $attributeValueRow['AttributeId'],$attributeValueRow['ProductId'],$attributeValueRow['Value']
            );
            $attributeValue->setAttributeName($attributeValueRow["Attribute"]);

            $attributesValues[] = $attributeValue;
        }
        return $attributesValues;
    }

    public function removeAllAttributesValues($productId)
    {
        $stmt = $this->conn->prepare("DELETE FROM attributevalues where ProductId = :productId;");
        $stmt->bindValue(':productId',  $productId);
        $stmt->execute();
    }

    public function addRating($rating)
    {
        $stmt = $this->conn->prepare(
            "INSERT into ratings(ProductId,Title,Description,Recommended,Rating,Userid,Approved) 
            values(:ProductId,:Title,:desc,:Recommended,:Rating,:Userid,:Approved)"
        );
        $stmt->bindValue(':ProductId',  intval($rating->getProductId()));
        $stmt->bindValue(':Title',  $rating->getTitle());
        $stmt->bindValue(':desc',  $rating->getDescription());
        $stmt->bindValue(':Recommended', intval($rating->getRecommended()), PDO::PARAM_INT);
        $stmt->bindValue(':Rating',  $rating->getRating());
        $stmt->bindValue(':Userid',  intval($rating->getUserid()), PDO::PARAM_INT);
        $stmt->bindValue(':Approved',  intval($rating->getApproved()), PDO::PARAM_INT);
        $stmt->execute();
    }
    
    public function approveRating($ratingId)
    {
        $stmt = $this->conn->prepare("UPDATE ratings SET Approved = 1 WHERE ratingId = :ratingId");
        $stmt->bindValue(":ratingId", $ratingId);
        $stmt->execute();
    }

    public function getAllRatingPaginated($page, $search, $pageSize )
    {
        $pageSize = !isset($pageSize) ? 5 : $pageSize;
        $skipNumber = !is_null($page) && $page > 0 ? ($pageSize * ($page - 1)) : 0;
        $stmt = null;
        $hasSearch = is_null($search) || $search === "" ? false :true;
        $whereClausule = $hasSearch ? " and p.Title like :search or pc.Title like :search or pc.Description like :search " : "";

        //contando total de registros...
        $stmt = $this->conn->prepare(
            "SELECT count(pc.RatingId) as Total from Ratings pc
            inner join products p on pc.ProductId = p.ProductId
            inner join users u on p.UserId = u.UserId where pc.Approved = 0 $whereClausule   "
        );
        if ($hasSearch) $stmt->bindValue(":search", '%' . $search . '%');
        $stmt->execute();
        $total = $stmt->fetch();
        $total = intval($total["Total"]);
        
        // fazendo consulta
        $stmt = $this->conn->prepare(
            "SELECT pc.RatingId, pc.ProductId,pc.Rating, pc.Recommended, pc.Title,pc.Description, pc.Approved, 
                    u.UserId , p.Title as ProductTitle,  p.Sku, u.Name as UserName, Image.filename as ImageDefault
            from Ratings pc
            inner join products p on pc.ProductId = p.ProductId
            inner join users u on p.UserId = u.UserId 
            left join (select pi.ProductId, pi.filename as filename from ProductsImages pi ) as Image on p.ProductId = Image.ProductId 
            where pc.Approved = 0 $whereClausule  group by pc.RatingId  order by p.title limit :pageSize OFFSET :skipNumber "
        );

        if ($hasSearch) $stmt->bindValue(":search", '%' . $search . '%');
        
        $stmt->bindValue(':pageSize', intval(trim($pageSize)), PDO::PARAM_INT);
        $stmt->bindValue(':skipNumber', intval(trim($skipNumber)), PDO::PARAM_INT);
        $stmt->execute();
        $produtosResult = $stmt->fetchAll();
        
        $ratings = array();
        foreach ($produtosResult as $rating) 
            $ratings[] = StatementHelper::ToRating($rating);

        return new PaginatedResults($ratings, $total, count($produtosResult), $page, $pageSize, "avaliacoes-pendentes?p=");
    }

    public function getAllRating($productId)
    {
        $stmt = $this->conn->prepare(
            "SELECT pc.RatingId, pc.ProductId, pc.Rating, pc.Recommended, pc.Title, pc.Description, 
                    pc.Approved, u.UserId, p.Title as ProductTitle,  p.Sku, u.Name as UserName
            from Ratings pc
            inner join products p on pc.ProductId = p.ProductId
            inner join users u on p.UserId = u.UserId             
            where pc.Approved = 1 and pc.ProductId = :ProductId group by pc.RatingId order by p.title "
        );
        $stmt->bindValue(':ProductId', intval($productId), PDO::PARAM_INT);
        $stmt->execute();
        $produtosResult = $stmt->fetchAll();
        
        $ratings = array();
        foreach ($produtosResult as $rating) 
            $ratings[] = StatementHelper::ToRating($rating);
        
        return $ratings;
    }

    public function getAllLikeds($userId)
    {
        $stmt = $this->conn->prepare(
            "SELECT p.ProductId, p.Title, p.Price, p.Description, p.CreatedAt, 
                    p.CreatedBy, p.Offer, p.Stock, p.Sku, Image.filename as ImageFileName,
                    p.UserId
            FROM Products p
            inner join productslikeds pl on p.productId = pl.productId
            left join (select pi.ProductId, pi.filename as filename from ProductsImages pi ) as Image on p.ProductId = Image.ProductId 
            where pl.userId = :userId  group by p.productid  order by p.title "
        );
        $stmt->bindValue(':userId',  $userId);
        $stmt->execute();
        $produtosResult = $stmt->fetchAll();

        $products = array();
        foreach ($produtosResult as $row) {
            $prod = StatementHelper::ToProduct($row);
            $products[] = $prod;
        }
        return $products;
    }


    public function getAllByPreviousOrders($userId)
    { 
        $stmt = $this->conn->prepare(
            "SELECT p.ProductId, p.Title, p.Price, p.Description, p.CreatedAt, 
                    p.CreatedBy, p.Offer, p.Stock, p.Sku, Image.filename as ImageFileName, p.UserId
            FROM Products p
            inner join productslikeds pl on p.productId = pl.productId
            left join (select pi.ProductId, pi.filename as filename from ProductsImages pi ) as Image on p.ProductId = Image.ProductId 
            where p.subCategoryId in(
                select p.SubCategoryId from orders o
                inner join orderitens oi on o.OrderId = oi.OrderId
                inner join Products p on oi.ProductId = p.ProductId 
                where o.userId = :userId group by p.SubCategoryId
            )  group by p.productid  order by p.title "
        );
        $stmt->bindValue(':userId',  $userId);
        $stmt->execute();
        $produtosResult = $stmt->fetchAll();
        $products = array();
        foreach ($produtosResult as $row) {
            $prod = StatementHelper::ToProduct($row);
            $products[] = $prod;
        }
        
        return $products;
    }
    
}