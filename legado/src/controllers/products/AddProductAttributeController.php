<?php

namespace controllers\products;

use controllers\IBaseController;
use domain\entities\Attribute;
use infra\repositories\AttributeRepository;
use infra\repositories\ProductRepository;
use services\SimilarProductService;

class AddProductAttributeController implements IBaseController
{
    private $_repository;

    public function __construct($factory)
    {
        $this->_repository = $factory->getAttributeRepository();
    }

    public function proccessRequest(): void
    {

        $attributes = $this->_repository->getAll();
        require $_SERVER['DOCUMENT_ROOT'] . '\\views\\admin\\product\\add-attribute.php';
    }
}
