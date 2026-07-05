<?php

namespace controllers;

use infra\helpers\SrcHelper;

class HomeController implements IBaseController
{
    private $_repoOfertas;
    private $_repoCarousel;
    private $_repoCategories;
    private $_repoProducts;
    public function __construct($factory)
    {
        $this->_repoOfertas = $factory->getProductOnOfferRepository();
        $this->_repoCarousel = $factory->getCarouselRepository();
        $this->_repoCategories = $factory->getCategoryRepository();
        $this->_repoProducts = $factory->getProductRepository();
    }

    public function getProductsOnOffer()
    {
        return $this->_repoOfertas->getAll();
    }

    public function getCaroselItens()
    {
        return $this->_repoCarousel->getAll();
    }

    public function proccessRequest(): void
    {
        $ofertas = $this->getProductsOnOffer();
        $caroselItens = $this->getCaroselItens();
        $categories = $this->_repoCategories->getAll();
        $carouselImgPath = SrcHelper::getCarouselSrc();
        
        $ofertasBaseadasCompras = [];
        
        if (isset($_SESSION["userId"]))
        {
            $ofertasBaseadasCompras = $this->_repoProducts->getAllByPreviousOrders($_SESSION["userId"]);
        }
        
        require "views/home/home.php";
    }
}
