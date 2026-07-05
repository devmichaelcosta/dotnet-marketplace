<?php

namespace controllers\users;

use controllers\IBaseController;

class UserEditController implements IBaseController
{
    private $_repoUser;
    private $_repoAddress;
    private $_repoStates;
    public function __construct($factory)
    {
        $this->_repoUser = $factory->getUserRepository();
        $this->_repoAddress = $factory->getAddressRepository();
        $this->_repoStates = $factory->getStateRepository();
    }


    public function proccessRequest(): void
    {
        $id = $_GET["id"];
        $user = $this->_repoUser->getById($id);
        if ($_SESSION["role"] == "vendedor" || $_SESSION["role"] == "comum") {
            if ($_SESSION["userId"] != $_GET["id"]) {
                header('location: /');
            }
        }

        $userAddresses = $this->_repoAddress->getAllByUserId($id);
        $states = $this->_repoStates->getAll();
        require "views/admin/users/editar-usuario.php";
    }
}
