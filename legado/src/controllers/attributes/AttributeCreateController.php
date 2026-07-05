<?php

namespace controllers\attributes;

use controllers\IBaseController;

class AttributeCreateController implements IBaseController
{
    public function __construct($factory)
    {
    }

    public function proccessRequest(): void
    {
        require "views/admin/attributes/create.php";
    }
}
