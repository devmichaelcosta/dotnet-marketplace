<?php

namespace controllers\attributes;

use controllers\IBaseController;

class AttributeListPartialController implements IBaseController
{
    private $_repoAttribute;

    public function __construct($factory)
    {
        $this->_repoAttribute = $factory->getAttributeRepository();
    }

    public function proccessRequest(): void
    {
        $page = 1;
        if (isset($_GET["p"]))
            $page = intval($_GET["p"]);

        $search = null;
        if (isset($_POST["s"]))
            $search = $_POST["s"];

        $paginatedResults = $this->_repoAttribute->getAllPaginated($page, $search, 5);
        $attributes = $paginatedResults->results;
        require "views/admin/attributes/list.php";
    }
}
