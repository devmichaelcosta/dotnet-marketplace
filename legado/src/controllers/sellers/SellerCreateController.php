<?php

namespace controllers\sellers;

use controllers\IBaseController;
use services\SellerService;

class SellerCreateController implements IBaseController
{
    private $_sellerService;

    public function __construct($factory)
    {
        $this->_sellerService = new SellerService($factory);
    }

    public function proccessRequest(): void
    {
        $model = $this->_sellerService->getCreateViewModel();
        require "views/admin/sellers/cadastrar.php";
    }
}
