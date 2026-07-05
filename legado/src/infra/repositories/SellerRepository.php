<?php

namespace infra\repositories;

use infra\MySqlRepository;
use infra\interfaces\ISellerRepository;
use models\Seller;
use models\PaginatedResults;
use PDO;
use infra\Logger;

class SellerRepository extends MySqlRepository implements ISellerRepository
{
    public function add($seller)
    {
        $stmt = $this->conn->prepare(
            "INSERT INTO Sellers 
                    (UserId,Age,Email,DateOfBirth,WebSite,Company,CNPJ,BranchOfActivity,FantasyName) 
                values 
                    (:UserId,:Age,:Email,:DateOfBirth,:WebSite,:Company,:CNPJ,:BranchOfActivity,:FantasyName)"
        );
        // var_dump($seller->getUserId());
        // var_dump($seller->getAge());
        // var_dump($seller->getCpf());
        $stmt->bindValue(':UserId', $seller->getUserId());
        $stmt->bindValue(':Age', $seller->getAge());
        $stmt->bindValue(':Email', $seller->getEmail());
        $stmt->bindValue(':DateOfBirth', $seller->getDateOfBirth());
        $stmt->bindValue(':WebSite', $seller->getWebSite());
        $stmt->bindValue(':Company', $seller->getCompany());
        $stmt->bindValue(':CNPJ', $seller->getCnpj());
        $stmt->bindValue(':BranchOfActivity',  $seller->getBranchOfActivity());
        $stmt->bindValue(':FantasyName', $seller->getFantasyName());
        if (!$stmt->execute()) {
            Logger::write("errosql");
            Logger::write("SellerRepository: " . $stmt->errorInfo());
        }
    }

    public function update($seller)
    {
        $stmt = $this->conn->prepare(
            "UPDATE Sellers 
             SET Age = :age, cpf = :cpf, email = :email, 
                 dateOfBirth = :dateOfBirth, webSite = :webSite,
                 company = :company, cnpj = :cnpj, branchOfActivity = :branchOfActivity, fantasyName = :fantasyName 
             WHERE sellerId = :sellerId"
        );

        $stmt->bindValue(':sellerId', $seller->getSellerId());
        $stmt->bindValue(':age', $seller->getAge());
        $stmt->bindValue(':cpf', $seller->getCpf());
        $stmt->bindValue(':email', $seller->getEmail());
        $stmt->bindValue(':dateOfBirth', $seller->getDateOfBirth());
        $stmt->bindValue(':webSite', $seller->getWebSite());
        $stmt->bindValue(':company', $seller->getCompany());
        $stmt->bindValue(':cnpj', $seller->getCnpj());
        $stmt->bindValue(':branchOfActivity',  $seller->getBranchOfActivity());
        $stmt->bindValue(':fantasyName', $seller->getFantasyName());

        if (!$stmt->execute()) {
            Logger::write("SellerRepository update: " . $stmt->errorInfo());
            return false;
        }
        return true;
    }

    public function total($search)
    {
        if (is_null($search) || $search === "") {
            $stmt = $this->conn->prepare(
                "SELECT count(SellerId) as total FROM Sellers "
            );


            $stmt->execute();
            $total = $stmt->fetch();
            return intval($total["total"]);
        } else {
            $stmt = $this->conn->prepare(
                "SELECT count(s.SellerId) total 
                    from Users u
                    inner join Sellers s on u.userid = s.userid
                    where 
                        u.role ='vendedor' and 
                        (
                            u.login like :search or 
                            u.name like :search or 
                            s.LastName like :search or 
                            s.City like :search or 
                            s.Company like :search or 
                            s.FantasyName like :search 
                        )"
            );
            $stmt->bindValue(":search", '%' . $search . '%');
            $stmt->execute();
            $total = $stmt->fetch();
            return intval($total["total"]);
        }
    }

    public function getAll($page, $search, $pageSize)
    {

        $skipNumber = 0;
        if (!is_null($page) && $page > 0)
            $skipNumber = $pageSize * ($page - 1);

        $total = $this->total($search);

        //echo "page: " . $page . "<br/> skipNumber: " . $skipNumber . "<br/>";

        //obtendo lista de usuarios...
        $usersResults = null;
        if (is_null($search) ||  $search === "") {
            $stmt = $this->conn->prepare(
                "SELECT 
                        s.SellerId, u.Name as Name, u.LastName as LastName, s.Company as Company, 
                        s.FantasyName as FantasyName,  u.Login as Login, u.userId as userId
                    from Users u
                    inner join Sellers s on u.userid = s.userid
                    where u.role = 'vendedor'
                    limit :pageSize OFFSET :skipNumber "
            );
            $stmt->bindValue(':pageSize', intval(trim($pageSize)), PDO::PARAM_INT);
            $stmt->bindValue(':skipNumber', intval(trim($skipNumber)), PDO::PARAM_INT);
            $stmt->execute();
            $usersResults = $stmt->fetchAll();
        } else {
            $stmt = $this->conn->prepare(
                "SELECT 
                        s.SellerId, u.Name as Name, u.LastName as LastName, s.Company as Company, 
                        s.FantasyName as FantasyName,  u.Login as Login, u.userId as userId
                    from Users u
                    inner join Sellers s on u.userid = s.userid
                    where 
                        u.role = 'vendedor' and 
                        (
                            u.login like :search or 
                            u.name like :search or 
                            u.LastName like :search or                             
                            s.Company like :search or 
                            s.FantasyName like :search 
                        ) 
                     limit :pageSize OFFSET :skipNumber "
            );
            $stmt->bindValue(":search", '%' . $search . '%');
            $stmt->bindValue(':pageSize', intval(trim($pageSize)), PDO::PARAM_INT);
            $stmt->bindValue(':skipNumber', intval(trim($skipNumber)), PDO::PARAM_INT);
            $stmt->execute();
            $usersResults = $stmt->fetchAll();
        }

        $sellers = array();
        foreach ($usersResults as $row) {
            $sellers[] = new Seller(
                $row["SellerId"],
                $row["Name"],
                $row["LastName"],
                $row["Company"],
                $row["FantasyName"],
                $row["Login"],
                $row["userId"]
            );
        }

        return  new PaginatedResults(
            $sellers,
            $total,
            count($sellers),
            $page,
            $pageSize,
            "/admin/vendedor?p="
        );
    }

    public function getById($sellerId)
    {
        $stmt = $this->conn->prepare(
            "SELECT 
                    s.SellerId as SellerId, u.Name as Name, u.LastName as LastName, s.Company as Company, 
                    s.FantasyName as FantasyName, u.Login as Login, u.userId as userId, s.Email as Email,
                    s.Website as Website, s.Cnpj as Cnpj, s.BranchOfActivity as BranchOfActivity,
                    u.Cpf as Cpf, s.age as Age,
                    s.DateOfBirth as DateOfBirth
                from Users u
                inner join Sellers s on u.userid = s.userid
                where  s.sellerId = :sellerId limit 1 "
        );
        $stmt->bindValue(":sellerId", $sellerId);
        $stmt->execute();
        $sellerResult = $stmt->fetch();

        $seller = null;
        if (isset($sellerResult)) {
            $seller = new Seller(
                $sellerResult["SellerId"],
                $sellerResult["Name"],
                $sellerResult["LastName"],
                $sellerResult["Company"],
                $sellerResult["FantasyName"],
                $sellerResult["Login"],
                $sellerResult["userId"]
            );
            $seller->setEmail($sellerResult["Email"]);
            $seller->setWebsite($sellerResult["Website"]);
            $seller->setCnpj($sellerResult["Cnpj"]);
            $seller->setBranchOfActivity($sellerResult["BranchOfActivity"]);
            $seller->setCpf($sellerResult["Cpf"]);
            $seller->setAge($sellerResult["Age"]);
            $seller->setDateOfBirth($sellerResult["DateOfBirth"]);
        }
        return $seller;
    }

    public function getByUserId($userId)
    {
        $stmt = $this->conn->prepare(
            "SELECT 
                    s.SellerId as SellerId, u.Name as Name, u.LastName as LastName, s.Company as Company, 
                    s.FantasyName as FantasyName, u.Login as Login, u.userId as userId, s.Email as Email,
                    s.Website as Website, s.Cnpj as Cnpj, s.BranchOfActivity as BranchOfActivity, 
                    u.Cpf as Cpf, s.age as Age,
                    s.DateOfBirth as DateOfBirth
                from Users u
                inner join Sellers s on u.userid = s.userid
                where  s.userId = :userId limit 1 "
        );
        $stmt->bindValue(":userId", $userId);
        $stmt->execute();
        $sellerResult = $stmt->fetch();

        $seller = null;
        if (isset($sellerResult)) {
            $seller = new Seller(
                $sellerResult["SellerId"],
                $sellerResult["Name"],
                $sellerResult["LastName"],
                $sellerResult["Company"],
                $sellerResult["FantasyName"],
                $sellerResult["Login"],
                $sellerResult["userId"]
            );
            $seller->setEmail($sellerResult["Email"]);
            $seller->setWebsite($sellerResult["Website"]);
            $seller->setCnpj($sellerResult["Cnpj"]);
            $seller->setBranchOfActivity($sellerResult["BranchOfActivity"]);
            $seller->setCpf($sellerResult["Cpf"]);
            $seller->setAge($sellerResult["Age"]);
            $seller->setDateOfBirth($sellerResult["DateOfBirth"]);
        }
        return $seller;
    }

    public function addSimplifiedSeller($userId)
    {
        $stmt = $this->conn->prepare(
            "INSERT INTO Sellers (userId) 
                values 
                (:userId)"
        );
        $stmt->bindValue(':userId', $userId);

        if (!$stmt->execute()) {
            Logger::write("SellerRepository->addSimplifiedSeller: " . $stmt->errorInfo());
        }
    }

    public function remove($sellerId)
    {
        $seller = $this->getById($sellerId);

        if (isset($seller)) {
            $userId = $seller->getUserId();
            $stmt = $this->conn->prepare(
                "delete from sellers where sellerId = :sellerId"
            );
            $stmt->bindValue(":sellerId", $sellerId);
            $stmt->execute();

            //removendo o endereço
            $stmt = $this->conn->prepare(
                "delete from address where userId = :userId"
            );
            $stmt->bindValue(":userId", $userId);
            $stmt->execute();

            //removendo itens de pedido
            $stmt = $this->conn->prepare(
                "delete from orderitens where productid in (select productid from products where userid = :userId);"
            );
            $stmt->bindValue(":userId", $userId);
            $stmt->execute();

            //removendo pedidos
            $stmt = $this->conn->prepare(
                "delete from orders where userid = :userId);"
            );
            $stmt->bindValue(":userId", $userId);
            $stmt->execute();

            //removendo produtos
            $stmt = $this->conn->prepare(
                "delete from products where userid = :userId;"
            );
            $stmt->bindValue(":userId", $userId);
            $stmt->execute();

            //removendo usuario
            $stmt = $this->conn->prepare(
                "delete from users where userid = :userId;"
            );
            $stmt->bindValue(":userId", $userId);
            $stmt->execute();
        }
    }
}
