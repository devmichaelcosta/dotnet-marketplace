<?php

namespace infra\repositories;

use infra\MySqlRepository;
use infra\interfaces\ICarouselRepository;
use PDO;

class CarouselRepository
extends MySqlRepository
implements ICarouselRepository
{
    public function getAll()
    {
        $query = "select CarouselImageId, FileName, `Order` from CarouselImages order by `order`";
        $resultado = $this->conn->query($query);
        $lista = $resultado->fetchAll();
        return $lista;
    }
}
