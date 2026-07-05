<?php

    namespace infra\repositories;    
    use infra\MySqlRepository;
    use infra\interfaces\ICartRepository;
    use PDO;
    use models\ProductCart;

    class CartRepository extends MySqlRepository implements ICartRepository 
    {
        public function addProduct($productId, $cartGroup)
        {
            $qtd = $this->getQtdProduct($productId, $cartGroup);
            echo "<br>qtd: " . $qtd;
            echo "<br>productId:". $productId ;
            echo "<br>cartGroup:". $cartGroup ;
            
            if ($qtd == 0)
            {
                echo "<br>inserindo";
                $stmt = $this->conn->prepare(
                    "INSERT INTO Carts (
                        ProductId, 
                        CartGroup, 
                        Qtd, 
                        CreatedAt
                    )  
                    values (:ProductId, :CartGroup, :Qtd, now())"
                );
                $stmt->bindValue(':ProductId', $productId);
                $stmt->bindValue(':CartGroup', $cartGroup);
                $stmt->bindValue(':Qtd', 1);
                $stmt->execute();
            } 
            else 
            { 
                echo "<br>atualizando";
                $qtd += 1;
                $stmt = $this->conn->prepare(
                    "UPDATE Carts SET
                    Qtd = :Qtd where ProductId = :ProductId and cartGroup = :CartGroup"
                );
                $stmt->bindValue(':ProductId', $productId);
                $stmt->bindValue(':CartGroup', $cartGroup);
                $stmt->bindValue(':Qtd', $qtd);
                $stmt->execute();
            }
        }

        public function getQtdProduct($productId, $cartGroup)
        {
            echo "getQtdProduct<br/>";
            echo "productId:". $productId . "<br/>";
            $qtd = 0;
            $stmt = $this->conn->prepare(
                "select qtd from Carts where CartGroup = :CartGroup and ProductId = :ProductId;"
            );
            $stmt->bindValue(':ProductId', $productId);
            $stmt->bindValue(':CartGroup', $cartGroup);
            $stmt->execute();
            $row = $stmt->fetch(PDO::FETCH_ASSOC);
            
            if ($row) 
                $qtd = $row['qtd'];
            
            return $qtd;
        }

        public function getProducts($cartGroup)
        {
            $stmt = $this->conn->prepare(
                "select c.CartId, c.CartGroup, p.ProductId, p.Title, p.Price,
                c.Qtd , (p.price * c.qtd) as SubTotal ,
                    (
                        select pImg.FileName from productsimages pImg 
                        where pImg.ProductId = p.ProductId limit 1
                    ) 
                    as Image
                from carts c
                inner join products p on c.productid = p.productid 
                where cartgroup = :CartGroup ;"
            );

            $stmt->bindValue(':CartGroup',$cartGroup);
            $stmt->execute();
            $result = $stmt->fetchAll();
            $products = array();
            foreach ($result as $product){
                $products[] = new ProductCart(
                    $product["CartId"],
                    $product["CartGroup"],
                    $product["ProductId"],
                    $product["Title"],
                    $product["Price"],
                    $product["Qtd"],
                    $product["Image"],
                    $product["SubTotal"]
                );
            }
            return $products;
        }
     
        public function getFinalPrice($cartGroup)
        {
            $price = 0;
            $stmt = $this->conn->prepare(
                "select sum(TotalPerItem) as Total from (
                    select c.cartgroup, p.title, p.price, c.qtd , (p.price * c.qtd) as TotalPerItem 
                    from carts c
                    inner join products p on c.productid = p.productid 
                    where cartgroup = :cartGroup
                ) as TotalCart;"
            );
            $stmt->bindValue(':cartGroup',$cartGroup);
            $stmt->execute();
            $row = $stmt->fetch(PDO::FETCH_ASSOC);
            if ($row)
                $price = $row["Total"];

            return $price;
        }
    }

?>