<?php

namespace controllers\attributes;

use controllers\IBaseController;

class AttributeListController  implements IBaseController
{
    private $_repoAttribute;

    public function __construct($factory)
    {
        $this->_repoAttribute = $factory->getAttributeRepository();
    }

    public function proccessRequest(): void
    {
        $page = isset($_GET["p"]) ? intval($_GET["p"]) : 1;
        $paginatedResults = $this->_repoAttribute->getAllPaginated($page, null, 5);
        $attributes = $paginatedResults->results;
        require "views/admin/attributes/index.php";
    }
}
