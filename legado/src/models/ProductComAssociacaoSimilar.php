<?php

namespace models;

class ProductComAssociacaoSimilar extends Product
{
    public function setAssociado($associado)
    {
        $this->associado = $associado;
    }

    private $associado;

    public function getAssociado()
    {
        return $this->associado;
    }
}
