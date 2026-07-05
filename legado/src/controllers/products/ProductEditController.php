<?php

namespace controllers\products;

use controllers\IBaseController;
use services\ProductService;

class ProductEditController implements IBaseController
{
    private $_productService;

    public function __construct($factory)
    {
        $this->_productService = new ProductService($factory);
    }

    public function proccessRequest(): void
    {
        $product = $this->_productService->getById($_GET['id']);
        $model = $this->_productService->getProductEditViewModel($_GET['id']);
       
        require $_SERVER['DOCUMENT_ROOT'] . '\\views\\admin\\product\\editar.php';
    }
}
