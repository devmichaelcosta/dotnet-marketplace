<?php

namespace controllers\sellers;

use controllers\IBaseController;
use services\SellerService;

class SellerRegisterController implements IBaseController
{
    private $_sellerService;

    public function __construct($factory)
    {
        $this->_sellerService = new SellerService($factory);
    }

    public function proccessRequest(): void
    {
        if (!isset($_SESSION["userId"])) {
            require "views/seller/register.php";
        } else {
            header('location: /admin/vendedor');
        }
    }
}
