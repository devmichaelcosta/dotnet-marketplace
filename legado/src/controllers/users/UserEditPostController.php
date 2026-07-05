<?php

namespace controllers\users;

use controllers\IBaseController;

use services\UserService;

class UserEditPostController implements IBaseController
{
    private $_userService;

    public function __construct($factory)
    {
        $this->_userService = new UserService($factory);
    }

    public function proccessRequest(): void
    {
        $retorno = $this->_userService->update();
        $retorno->urlDestino = "/admin/usuario";
        if ($_SESSION["role"] == "vendedor" || $_SESSION["role"] == "comum")
            $retorno->urlDestino = "/admin/usuario/minhas-compras";

        echo json_encode($retorno);
    }
}
