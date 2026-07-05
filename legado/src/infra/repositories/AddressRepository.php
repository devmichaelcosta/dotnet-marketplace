<?php

namespace infra\repositories;

use infra\MySqlRepository;
use infra\interfaces\IAddressRepository;
use models\Address;
use models\PaginatedResults;
use PDO;

class AddressRepository
extends MySqlRepository
implements IAddressRepository
{
    public function add($address)
    {
        $stmt = $this->conn->prepare(
            "insert into addresses (street, cep, neighborhood, city, stateid, complement,userId)
                values(:street,:cep,:neighborhood,:city,:stateid,:complement,:userId);"
        );
        $stmt->bindValue(':street', $address->getStreet());
        $stmt->bindValue(':cep', $address->getCep());
        $stmt->bindValue(':neighborhood', $address->getNeighborhood());
        $stmt->bindValue(':city', $address->getCity());
        $stmt->bindValue(':stateid', intval($address->getStateId()));
        $stmt->bindValue(':complement', $address->getComplement());
        $stmt->bindValue(':userId', intval($address->getUserId()));
        $stmt->execute();

        return $this->conn->lastInsertId();
    }

    public function removeAllByUserId($userId)
    {
        $stmt = $this->conn->prepare(
            "delete from addresses where userId = :userId "
        );
        $stmt->bindParam(':userId', $userId);
        $stmt->execute();
    }

    public function remove($addressId)
    {
        $stmt = $this->conn->prepare(
            "delete from addresses where addressId = :addressId "
        );
        $stmt->bindParam(':addressId', $addressId);
        $stmt->execute();
    }

    public function update($address)
    {
        $stmt = $this->conn->prepare(
            "UPDATE addresses 
                    set 
                        street = :street, 
                        cep = :cep,
                        neighborhood = :neighborhood, 
                        city = :city,
                        stateid = :stateid, 
                        complement = :complement,
                        userId = :userId
                 where addressId = :addressId"
        );

        $stmt->bindValue(':street', $address->getStreet());
        $stmt->bindValue(':cep', $address->getCep());
        $stmt->bindValue(':neighborhood', $address->getNeighborhood());
        $stmt->bindValue(':city', $address->getCity());
        $stmt->bindValue(':stateid', $address->getStateId());
        $stmt->bindValue(':complement', $address->getComplement());
        $stmt->bindValue(':userId', $address->getUserId());
        $stmt->bindValue(':addressId', $address->getAddressId());
        $stmt->execute();

        return $this->conn->lastInsertId();
    }

    public function getAllByUserId($userId)
    {
        $stmt = $this->conn->prepare(
            "SELECT * from addresses where userId = :userId"
        );
        $stmt->bindValue(':userId', $userId);
        $stmt->execute();
        $addressesResults = $stmt->fetchAll();

        //colocando resultados em um array de objetos
        $addressesArray = array();
        foreach ($addressesResults as $row) {
            $addressesArray[] = new Address(
                $row["AddressId"],
                $row["UserId"],
                $row["Street"],
                $row["CEP"],
                $row["Neighborhood"],
                $row["City"],
                $row["StateId"],
                $row["Complement"]
            );
        }

        return $addressesArray;
    }

    public function getFirstByUserId($userId)
    {
        $stmt = $this->conn->prepare(
            "SELECT * from addresses where userId = :userId LIMIT 1"
        );
        $stmt->bindValue(':userId', $userId);
        $stmt->execute();
        $row = $stmt->fetch(PDO::FETCH_ASSOC);

        $address = null;
        if ($row)
            $address = new Address(
                $row["AddressId"],
                $row["UserId"],
                $row["Street"],
                $row["CEP"],
                $row["Neighborhood"],
                $row["City"],
                $row["StateId"],
                $row["Complement"]
            );

        return $address;
    }
}
