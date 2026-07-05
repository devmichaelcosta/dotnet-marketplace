<?php

namespace controllers\users;

use controllers\IBaseController;
use models\JsonSuccess;
use models\JsonError;
use models\User;

class UserCreatePostController implements IBaseController
{
    private $_repoUser;

    public function __construct($factory)
    {
        $this->_repoUser = $factory->getUserRepository();
    }

    public function proccessRequest(): void
    {
        //removendo possiveis tags e scripts...
        $login = filter_input(INPUT_POST, 'login', FILTER_SANITIZE_STRING);
        $password = filter_input(INPUT_POST, 'password', FILTER_SANITIZE_STRING);
        $name = filter_input(INPUT_POST, 'name', FILTER_SANITIZE_STRING);
        $cpf = filter_input(INPUT_POST, 'cpf', FILTER_SANITIZE_STRING);
        $lastName = filter_input(INPUT_POST, 'lastName', FILTER_SANITIZE_STRING);
        $role = filter_input(INPUT_POST, 'role', FILTER_SANITIZE_STRING);
        $user = new User(null, $login, trim($password), $name, $role, $cpf, $lastName);


        //validando modelo se valido retornamos um JSON.
        if ($user->isValid()) {
            $this->_repoUser->add($user);
            //header('Location: lista-usuarios');
            $retorno = new JsonSuccess("Usuário cadastrado com sucesso");
            header('Content-type:application/json;charset=utf-8');
        } else
            $retorno = new JsonError("Não foi possivel cadastrar o usuário");

        echo json_encode($retorno);
    }
}
