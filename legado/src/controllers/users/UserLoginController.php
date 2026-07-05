<?php

namespace controllers\users;

use controllers\IBaseController;

class UserLoginController implements IBaseController
{
    //private $_repoSeed;

    public function __construct($factory)
    {
        //$this->_repoSeed = $factory->getSeedRepository();
    }

    public function proccessRequest(): void
    {
        // if(!isset($_SESSION["userId"])) {
        //     echo 'deslogado';
        // } else{
        //     echo 'logado';
        // }
        require "views/usuario/login.php";
    }
}
