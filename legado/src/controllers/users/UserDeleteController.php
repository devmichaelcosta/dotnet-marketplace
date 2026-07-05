<?php

namespace controllers\users;

use controllers\IBaseController;
use models\JsonSuccess;
use models\JsonError;

class UserDeleteController implements IBaseController
{
    private $_repoUser;

    public function __construct($factory)
    {
        $this->_repoUser = $factory->getUserRepository();
    }

    public function proccessRequest(): void
    {
        try {
            $id = $_POST["id"];
            if (is_null($id) || $id === false) {
                $retorno = new JsonError("Não foi possivel encontrar o usuário");
                header('Content-type:application/json;charset=utf-8');
                echo json_encode($retorno);
            } else {

                $this->_repoUser->remove($_POST["id"]);
                $retorno = new JsonSuccess("Usuário removido com sucesso.");
                header('Content-type:application/json;charset=utf-8');
                echo json_encode($retorno);
            }
        } catch (Exception $e) {
            $retorno = new JsonError("Não foi possivel cadastrar o usuário");
            header('Content-type:application/json;charset=utf-8');
            echo json_encode($retorno);
        }
    }
}
