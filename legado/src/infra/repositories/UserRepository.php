<?php

    namespace infra\repositories;    
    use infra\MySqlRepository;
    use infra\interfaces\IUserRepository;
    use models\User;
    use models\Seller;
    use models\PaginatedResults;
    use PDO;

    class UserRepository 
        extends MySqlRepository 
        implements IUserRepository 
    {
        public function add($user)
        {
            $stmt = $this->conn->prepare(
                "INSERT INTO Users (Login, Password, Name, Role, cpf,lastName) 
                values 
                (:loginParam,:passwordParam, :nameParam,:role,:cpf,:lastName)"
            );
            $stmt->bindValue(':loginParam', $user->getLogin());
            $stmt->bindValue(':passwordParam', password_hash($user->getPassword(), PASSWORD_ARGON2I));
            $stmt->bindValue(':nameParam',$user->getName());
            $stmt->bindValue(':role',$user->getRole());
            $stmt->bindValue(':lastName',$user->getLastName());
            $stmt->bindValue(':cpf',$user->getCpf());
            
            if (!$stmt->execute())
            {
                //print_r($stmt->errorInfo());
                return null;
            }
            return $this->conn->lastInsertId();
        }

        public function remove($id)
        {
            $stmt = $this->conn->prepare(
                "delete from productsimages where productid in (select productid from products where userid = :id)"
            );
            $stmt->bindParam(':id', $id);
            $stmt->execute();
            
            $stmt = $this->conn->prepare("delete from products where UserId = :id");
            $stmt->bindParam(':id', $id);
            $stmt->execute();

            $stmt = $this->conn->prepare("delete from sellers where UserId = :id");
            $stmt->bindParam(':id', $id);
            $stmt->execute();

            $stmt = $this->conn->prepare("delete from Users where UserId = :id");
            $stmt->bindParam(':id', $id);
            $stmt->execute();
        }

        public function altera($user)
        {
            $stmt = $this->conn->prepare(
                "UPDATE Users 
                    set 
                        login = :loginParam, 
                        name = :nameParam,
                        role = :role,
                        cpf = :cpf,
                        lastName = :lastName
                 where UserId = :userId");
            
            $stmt->bindValue(':userId', $user->getUserId());
            $stmt->bindValue(':loginParam', $user->getLogin());
            $stmt->bindValue(':nameParam', $user->getName());
            $stmt->bindValue(':cpf', $user->getCpf());
            $stmt->bindValue(':lastName', $user->getLastName());
            $stmt->bindValue(':role', $user->getRole());
            $stmt->execute(); 
        }

        public function getById($id)
        {
            $stmt = $this->conn->prepare(
                "SELECT UserId, Login, Name, Password, Role, Cpf, LastName FROM Users 
                 WHERE UserId = :UserId LIMIT 1"
            );
            $stmt->bindValue(':UserId', $id);
            $stmt->execute();
            $row = $stmt->fetch(PDO::FETCH_ASSOC);
            $usuario = null;
            if ($row) 
                $usuario = new User(
                    $row['UserId'], 
                    $row['Login'], 
                    $row['Password'], 
                    $row['Name'],
                    $row['Role'],
                    $row['Cpf'],
                    $row['LastName']
                );
          
            return $usuario;
        }

        public function getByLogin($login)
        {
            $usuario = null;
            $stmt = $this->conn->prepare(
                "SELECT UserId, Login, Name, Password, Role, Cpf, LastName FROM Users 
                 WHERE login = :login LIMIT 1"
            );
            $stmt->bindValue(':login', $login);
            $stmt->execute();
            $row = $stmt->fetch(PDO::FETCH_ASSOC);
            if ($row) 
                $usuario = new User(
                    $row['UserId'],
                    $row['Login'], 
                    $row['Password'], 
                    $row['Name'], 
                    $row['Role'],
                    $row['Cpf'],
                    $row['LastName']
                );
          
            return $usuario;
        }

        public function getSellers()
        {
            $stmt = $this->conn->prepare(
                "SELECT s.sellerId as sellerId, u.userId as userId, u.name as name, u.lastName as lastName, 
                        s.company as company, s.fantasyName as fantasyName,  u.login as login
                 FROM Users as u 
                 inner join Sellers s on u.userId = s.userId
                 where u.role = 'vendedor' "
            );
            
            $stmt->execute();
            $results = $stmt->fetchAll();

            $sellers = array();
            foreach($results as $seller){
                $sellers[] = new Seller(
                    $seller["sellerId"],
                    $seller["name"],
                    $seller["lastName"],
                    $seller["company"],
                    $seller["fantasyName"], 
                    $seller["login"],
                    $seller["userId"]
                );
            }

            return $sellers;
        }


        public function total($search)
        {
            if (is_null($search) ||  $search === "")
            {
                $stmt = $this->conn->prepare(
                    "SELECT count(UserId) as total FROM Users "
                );
                $stmt->execute();
                $total = $stmt->fetch();
                return intval($total["total"]);
            }
            else
            {
                $stmt = $this->conn->prepare( 
                    "SELECT count(UserId) as total FROM Users 
                     WHERE Login like :search or Name like :search" 
                );
                $stmt->bindValue(":search", '%' . $search . '%');
                $total = $stmt->fetch();
                return intval($total["total"]);
            }  
        }

        public function getAll($page, $search, $pageSize)
        {
            //configurando variaveis para paginação
            if (!isset($page))
                $page = 0;
            
            if (!isset($pageSize))
                $pageSize = 5;  
            
            $skipNumber = 0;
            if (!is_null($page) && $page > 0)
                $skipNumber = $pageSize * ($page - 1);
            
            $total = $this->total($search);
            //echo "page: " . $page . "<br/> skipNumber: " . $skipNumber . "<br/>";

            //obtendo lista de usuarios...
            $usersResults = null;
            if (is_null($search) ||  $search === "")
            {
                $stmt = $this->conn->prepare(
                    "SELECT UserId, Login, Name, Role 
                     FROM Users limit :pageSize OFFSET :skipNumber "
                );
                $stmt->bindValue(':pageSize', intval(trim($pageSize)), PDO::PARAM_INT);
                $stmt->bindValue(':skipNumber', intval(trim($skipNumber)), PDO::PARAM_INT);
                $stmt->execute();
                $usersResults = $stmt->fetchAll();
            }
            else
            {
                $stmt = $this->conn->prepare( 
                    "SELECT UserId, Login, Name, Role FROM Users 
                     WHERE Login like :search or 
                     Name like :search order by name 
                     limit :pageSize OFFSET :skipNumber " 
                );
                $stmt->bindValue(":search", '%' . $search . '%');
                $stmt->bindValue(':pageSize', intval(trim($pageSize)), PDO::PARAM_INT);
                $stmt->bindValue(':skipNumber', intval(trim($skipNumber)), PDO::PARAM_INT);
                $stmt->execute();
                $usersResults = $stmt->fetchAll();
            }   
            
            return new PaginatedResults(
                $usersResults, 
                $total, 
                count($usersResults),
                $page,
                $pageSize,
                "/admin/usuario?p="
            );
;
        }
    }
