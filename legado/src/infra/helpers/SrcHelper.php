<?php

namespace infra\helpers;

class SrcHelper
{
    public static function getCarouselSrc()
    {
        return "/assets/img/carousel/";
    }

    public static function getDefaultSearchSrc()
    {
        return "/assets/js/models/";
    }

    public static function getProductImg()
    {
        return isset($_SERVER['REQUEST_SCHEME'])
            ? $_SERVER['REQUEST_SCHEME'] . "/assets/img/products/"
            : "/assets/img/products/";
    }

    public static function getCategoryImgPhysicPath()
    {
        return $_SERVER['DOCUMENT_ROOT'] . "/assets/img/categories/";
    }

    public static function getProductImgPhysicPath()
    {
        return $_SERVER['DOCUMENT_ROOT'] . "/assets/img/products/";
    }

    public static function getCategoryImg()
    {
        return "/assets/img/categories/";
    }

    public static function getProductAdminJs()
    {
        return "/assets/js/admin/product/";
    }
}
