<?php

namespace controllers\sellers;

use controllers\IBaseController;
use services\SellerService;
use infra\Logger;

class SellerEditController implements IBaseController
{
    private $_sellerService;

    public function __construct($factory)
    {
        $this->_sellerService = new SellerService($factory);
    }

    public function proccessRequest(): void
    {
        $model = $this->_sellerService->getEditViewModel();
        if ($_SESSION["role"] == "vendedor") {
            if ($_SESSION["sellerId"] != $_GET["id"]) {
                Logger::write("Usuário tentou entrar em vendedor/edit e não devia. ");
                header("location: /login");
            }
        } else if ($_SESSION["role"] == "comun") {
            if ($_SESSION["userId"] != $_GET["id"]) {
                Logger::write("Usuário tentou entrar em vendedor/edit e não devia. ");
                header("location: /login");
            }
        }
        require "views/admin/sellers/editar.php";
    }
}
