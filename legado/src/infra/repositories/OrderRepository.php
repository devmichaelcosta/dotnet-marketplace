<?php

    namespace infra\repositories;    
    use infra\MySqlRepository;
    use infra\interfaces\IOrderRepository;
    use models\Order;
    use models\OrderItem;
    use models\OrderDetailsViewModel;
    use models\PaginatedResults;
    use models\ProductOrder;
    use PDO;
    use PDOException;

    class OrderRepository  extends MySqlRepository  implements IOrderRepository
    {
        public function add($order)
        {
            try{    
               
                $stmt = $this->conn->prepare(
                    "INSERT INTO orders
                    (
                        total, createdat, userId, stateId, cardOwnerName,
                        expirationDate, name, address, neighborhood, cep, 
                        city, cpf, complement
                    )
                    values
                    (
                        :total, now(), :userId, :stateId, :cardOwnerName,
                        :expirationDate,:name,:address,:neighborhood,:cep,:city,:cpf,:complement
                    );"
                );
                $stmt->bindValue(':total', $order->getTotal());
                $stmt->bindValue(':userId', intval($order->getUserId()));
                $stmt->bindValue(':stateId', intval($order->getStateId()));
                $stmt->bindValue(':cardOwnerName', $order->getCardOwner());
                $stmt->bindValue(':expirationDate', $order->getExpirationDate());
                $stmt->bindValue(':name', $order->getName());
                $stmt->bindValue(':address', $order->getAddress());
                $stmt->bindValue(':neighborhood', $order->getNeighborhood());
                $stmt->bindValue(':cep', $order->getCep());
                $stmt->bindValue(':city', $order->getCity());
                $stmt->bindValue(':cpf', $order->getCpf());
                $stmt->bindValue(':complement', $order->getComplement());
                $stmt->execute();
                $orderId = $this->conn->lastInsertId();
              
                foreach($order->getOrderItens() as $item)
                {
                    $stmt = $this->conn->prepare(
                        
                        "insert into orderitens(orderid,productid,qtd) 
                        values(
                            :orderId, :productId, :qtd
                        )"
                    );
                    $stmt->bindValue(':orderId', intval($orderId));
                    $stmt->bindValue(':productId', $item->getProductId());
                    $stmt->bindValue(':qtd', $item->getQtd());
                   // echo "orderId: ". $orderId . ", productid: " . $item->getProductId() .", qtd:" .$item->getQtd() . "<br>";
                    $stmt->execute();
                }
                
                return $orderId;
                
            }
            catch(PDOException $Exception){
                echo "erro " .  $Exception->getMessage();
                return null;
            }
        }

        public function getOrderWithProducts($orderId)
        {
            $stmt = $this->conn->prepare(
                "SELECT o.OrderId as OrderId, o.Total as Total, o.Name as Name, o.CreatedAt as CreatedAt,
                o.Address as Address,o.Neighborhood as Neighborhood,o.CEP as CEP,o.CPF as CPF,
                o.City as City,o.Complement as Complement,s.stateAbreviattion as State
                FROM Orders o
                left join States s on o.stateid = s.stateid
                WHERE o.orderId = :orderId LIMIT 1"
            );
            $stmt->bindValue(':orderId', $orderId);
            $stmt->execute();
            $row = $stmt->fetch(PDO::FETCH_ASSOC);
            $order = null;

            if ($row){
                $order = new OrderDetailsViewModel(
                    $row["OrderId"], 
                    $row["CreatedAt"], 
                    $row["Total"]
                );
                
                $order->setAddress($row["Address"])
                      ->setName($row["Name"])
                      ->setNeighborhood($row["Neighborhood"])
                      ->setCep($row["CEP"])
                      ->setCity($row["City"])
                      ->setCpf($row["CPF"])
                      ->setComplement($row["Complement"])
                      ->setState($row["State"]);

                $productsOrder = $this->getOrderItens($orderId);
                $order->setProducts($productsOrder);
            }
            return $order;
        }

        public function getOrderItens($orderId)
        {
            $stmt = $this->conn->prepare(
                "SELECT p.ProductId as ProductId, p.UserId as UserId, 
                        p.Title as Title, u.name as Seller,
                        oi.qtd as Qtd, p.Price as Price, (oi.qtd * p.price) as SubTotal
                from orderitens oi
                inner join products p on oi.productid = p.productid
                inner join users u on p.userid = u.userid
                where oi.orderid = :orderId"
            );
            $stmt->bindValue(':orderId', $orderId);
            $stmt->execute();
            $results = $stmt->fetchAll();

            $productsOrder = array();
            foreach($results as $row)
            {
                $productsOrder[] = new ProductOrder(
                    $orderId, 
                    $row["ProductId"], 
                    $row["UserId"],
                    $row["Title"], 
                    $row["Seller"],
                    $row["Qtd"],
                    $row["Price"],
                    $row["SubTotal"]
                );
            }

            return $productsOrder;
        }

        public function getById($orderId)
        {
            $stmt = $this->conn->prepare(
                "SELECT orderId, total, name 
                 FROM Orders
                 WHERE orderId = :orderId LIMIT 1"
            );
            $stmt->bindValue(':orderId', $orderId);
            $stmt->execute();

            $row = $stmt->fetch(PDO::FETCH_ASSOC);
            if ($row) 
                $usuario = new Order(
                    $row['orderId'], 
                    $row['total'], 
                    $row['name']
                );
          
            return $usuario;
        }

        public function total($search,$userId)
        {
            if (is_null($search) ||  $search === "")
            {
                $stmt = $this->conn->prepare(
                    "SELECT count(OrderId) as total FROM Orders where userId = :userId "
                );
                $stmt->bindValue(':userId', intval($userId), PDO::PARAM_INT);
                $stmt->execute();
                $total = $stmt->fetch();
                return intval($total["total"]);
            }
            else
            {
                $stmt = $this->conn->prepare( 
                    "SELECT count(OrderId) as total FROM Orders 
                     WHERE 
                        userId = :userId and (
                        CardOwnerName like :search or 
                        Name like :search or 
                        Address like :search or 
                        Complement like :search )
                        "
                );
                $stmt->bindValue(':userId', intval($userId), PDO::PARAM_INT);
                $stmt->bindValue(":search", '%' . $search . '%');
                $total = $stmt->fetch();
                return intval($total["total"]);
            }  
        }

        public function getAll($page, $search, $pageSize, $userId)
        {
            //configurando variaveis para paginação
            if (!isset($page))
                $page = 0;
            
            if (!isset($pageSize))
                $pageSize = 5;  
            
            $skipNumber = 0;
            if (!is_null($page) && $page > 0)
                $skipNumber = $pageSize * ($page - 1);
            
            $total = $this->total($search,$userId);
            //echo "page: " . $page . "<br/> skipNumber: " . $skipNumber . "<br/>";

            //obtendo lista de usuarios...
            $ordersResults = null;
            if (is_null($search) ||  $search === "")
            {
                $stmt = $this->conn->prepare(
                    "SELECT 
                        o.orderId as orderId,
                        o.userId as userId, 
                        o.total as total,
                        o.cardOwnerName as cardOwnerName,
                        s.stateId as stateId,
                        o.name as name, 
                        o.cep as cep, 
                        o.complement as complement,
                        o.address as address, 
                        o.neighborhood as neighborhood,
                        o.city as city,
                        o.createdAt as createdAt
                     FROM Orders o
                     left join states s on o.stateid = s.stateid
                     where o.userId = :userId 
                     order by o.createdat desc
                     limit :pageSize OFFSET :skipNumber "
                );
                $stmt->bindValue(':pageSize', intval(trim($pageSize)), PDO::PARAM_INT);
                $stmt->bindValue(':skipNumber', intval(trim($skipNumber)), PDO::PARAM_INT);
                $stmt->bindValue(':userId', intval($userId), PDO::PARAM_INT);
                
                $stmt->execute();
                $ordersResults = $stmt->fetchAll();
            }
            else
            {
                $stmt = $this->conn->prepare( 
                    "SELECT o.orderId as orderId, 
                            o.userId as userId,
                            o.total as total,
                            o.cardOwnerName as cardOwnerName
                            o.name as name,
                            o.cep as cep,
                            o.address as address,
                            o.neighborhood,
                            o.city,
                            o.createdAt as createdAt
                     FROM Orders o
                     left join states s on o.stateid = s.stateid
                     WHERE 
                        o.userId = :userId and (
                        o.Name like :search or 
                        o.City like :search or 
                        o.Address like :search or 
                        o.Complement like :search or
                        s.stateAbreviattion like :search ) 
                     order by o.createdat desc 
                     
                     limit :pageSize OFFSET :skipNumber " 
                );
                $stmt->bindValue(":search", '%' . $search . '%');
                $stmt->bindValue(':pageSize', intval(trim($pageSize)), PDO::PARAM_INT);
                $stmt->bindValue(':skipNumber', intval(trim($skipNumber)), PDO::PARAM_INT);
                $stmt->bindValue(':userId', intval($userId), PDO::PARAM_INT);
                $stmt->execute();
                $ordersResults = $stmt->fetchAll();
            }
            
            $orders = array();
            foreach($ordersResults as $row)
            {
                $order =new Order(
                    $row["orderId"], 
                    $row["userId"],
                    $row["total"],
                    $row["name"],
                    $row["cardOwnerName"],
                    null,
                    null,
                    null,
                    $row["cep"],
                    null,
                    $row["address"],
                    $row["neighborhood"],
                    $row["city"],
                    $row["stateId"],
                    $row["complement"],
                    null
                );
                $order->setCreatedAt($row["createdAt"]);
                $orders[] = $order;
            }
            
            return new PaginatedResults(
                $orders, $total, count($orders), $page, 
                $pageSize, "/admin/usuario/minhas-compras?p="
            );
        }

        public function delete($id)
        {
            $stmt = $this->conn->prepare(
                "delete from orderitens where orderId = :orderId"
            );
            $stmt->bindValue(':orderId', intval($id));
            $stmt->execute();

            $stmt = $this->conn->prepare(
                "delete from orders where orderId = :orderId"
            );
            $stmt->bindValue(':orderId', intval($id));
            $stmt->execute();
        }
    }
