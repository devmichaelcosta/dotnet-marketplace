<?php

namespace controllers\users;

use controllers\IBaseController;
use services\UserService;

class UserRegisterController implements IBaseController
{
    private $_sellerService;

    public function __construct($factory)
    {
        $this->_sellerService = new UserService($factory);
    }

    public function proccessRequest(): void
    {
        if (!isset($_SESSION["userId"]))
            require "views/usuario/registrar.php";
        else
            header('location: /');
    }
}
