<?php

namespace controllers\users;

use controllers\IBaseController;

class UserListController implements IBaseController
{
    private $_repoUser;

    public function __construct($factory)
    {
        $this->_repoUser = $factory->getUserRepository();
    }

    public function proccessRequest(): void
    {
        $page = 1;
        if (isset($_GET["p"]))
            $page = intval($_GET["p"]);

        $search = null;
        if (isset($_GET["s"]))
            $search = $_GET["s"];

        $paginatedResults = $this->_repoUser->getAll($page, $search, 5);
        $users = $paginatedResults->results;
        require "views/admin/users/lista-usuario.php";
    }
}
