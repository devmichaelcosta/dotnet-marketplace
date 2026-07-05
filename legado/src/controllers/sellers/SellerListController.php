<?php

namespace controllers\sellers;

use controllers\IBaseController;

use services\SellerService;

class SellerListController implements IBaseController
{
    private $_sellerService;

    public function __construct($factory)
    {
        $this->_sellerService = new SellerService($factory);
    }

    public function proccessRequest(): void
    {
        $paginatedResults = $this->_sellerService->getAllPaginated();
        $sellers = $paginatedResults->results;
        require "views/admin/sellers/lista.php";
    }
}
