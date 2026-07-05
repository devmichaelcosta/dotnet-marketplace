<?php

namespace controllers\attributes;

use controllers\IBaseController;

class AttributeEditController implements IBaseController
{
    private $_repoAttribute;

    public function __construct($factory)
    {
        $this->_repoAttribute = $factory->getAttributeRepository();
    }

    public function proccessRequest(): void
    {
        $id = $_GET["id"];
        $attribute = $this->_repoAttribute->getById($id);
        require "views/admin/attributes/edit.php";
    }
}
