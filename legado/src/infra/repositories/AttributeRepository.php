<?php

namespace infra\repositories;

use infra\MySqlRepository;
use domain\entities\Attribute;
use domain\repositories\IAttributeRepository;
use models\PaginatedResults;
use PDO;

class AttributeRepository  extends MySqlRepository implements IAttributeRepository
{
    public function add($category)
    {
        $stmt = $this->conn->prepare("INSERT INTO Attributes (name)  values  (:name)");
        $stmt->bindValue(':name', $category->getName());
        $stmt->execute();

        return $this->conn->lastInsertId();
    }

    public function remove($attributeId)
    {
        $stmt = $this->conn->prepare("delete from Attributes where attributeId = :attributeId ");
        $stmt->bindParam(':attributeId', $attributeId);
        $stmt->execute();
    }

    public function update($category)
    {
        $stmt = $this->conn->prepare("UPDATE Attributes set name = :name where attributeId = :attributeId");
        $stmt->bindValue(':name', $category->getName());
        $stmt->bindValue(':attributeId', $category->getAttributeId());
        $stmt->execute();
    }

    public function getById($id)
    {
        $stmt = $this->conn->prepare("SELECT attributeId, name from Attributes WHERE attributeId = :attributeId LIMIT 1");
        $stmt->bindValue(':attributeId', intval($id));
        $stmt->execute();
        $row = $stmt->fetch(PDO::FETCH_ASSOC);

        $attribute = null;

        if ($row)
            $attribute = new Attribute($row['attributeId'], $row['name']);

        return $attribute;
    }

    public function total($search)
    {
        $stmt = null;
        if (is_null($search) ||  $search === "") {
            $stmt = $this->conn->prepare("SELECT count(attributeId) as qtd FROM Attributes ");
        } else {
            $stmt = $this->conn->prepare("SELECT  count(attributeId)  as qtd FROM Attributes WHERE name like :search  ");
            $stmt->bindValue(":search", '%' . $search . '%');
        }
        $stmt->execute();
        $total = $stmt->fetch();
        return intval($total["qtd"]);
    }

    public function getAll()
    {
        $attributesResults = null;
        $stmt = $this->conn->prepare("SELECT attributeId,name FROM Attributes order by name");
        $stmt->execute();
        $attributesResults = $stmt->fetchAll();

        $attributesArray = array();
        foreach ($attributesResults as $row)
            $attributesArray[] = new Attribute($row["attributeId"], $row["name"]);

        return $attributesArray;
    }

    public function getAllPaginated($page, $search, $pageSize)
    {
        $page = !isset($page) ? 0 : $page;
        $pageSize = !isset($pageSize) ? 5 : $pageSize;
        $skipNumber = (!is_null($page) && $page > 0) ? $pageSize * ($page - 1) : 0;
        $total = $this->total($search);
        $total = isset($total) ? $total : 0;
        $attributesResults = null;

        if (is_null($search) ||  $search === "") {
            $stmt = $this->conn->prepare("SELECT attributeId, name FROM Attributes limit :pageSize OFFSET :skipNumber ");
            $stmt->bindValue(':pageSize', intval(trim($pageSize)), PDO::PARAM_INT);
            $stmt->bindValue(':skipNumber', intval(trim($skipNumber)), PDO::PARAM_INT);
            $stmt->execute();
            $attributesResults = $stmt->fetchAll();
        } else {
            $stmt = $this->conn->prepare(
                "SELECT attributeId, name FROM Attributes WHERE name like :search  order by name limit :pageSize OFFSET :skipNumber "
            );
            $stmt->bindValue(":search", '%' . $search . '%');
            $stmt->bindValue(':pageSize', intval(trim($pageSize)), PDO::PARAM_INT);
            $stmt->bindValue(':skipNumber', intval(trim($skipNumber)), PDO::PARAM_INT);
            $stmt->execute();
            $attributesResults = $stmt->fetchAll();
        }

        $attributeArray = array();
        foreach ($attributesResults as $row)
            $attributeArray[] = new Attribute($row["attributeId"], $row["name"]);

        return new PaginatedResults(
            $attributeArray,
            $total,
            count($attributeArray),
            $page,
            $pageSize,
            "/admin/atributo?p="
        );
    }
}
