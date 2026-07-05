<?php

namespace controllers\users;

use controllers\IBaseController;
use services\UserService;
use models\requests\UserRegisterRequest;
use models\JsonSuccess;
use models\JsonError;

class UserRegisterPostController implements IBaseController
{
    private $_userService;

    public function __construct($factory)
    {
        $this->_userService = new UserService($factory);
    }

    public function proccessRequest(): void
    {
        $retornoJson = null;

        $request = new UserRegisterRequest(
            $_POST["name"],
            $_POST["lastName"],
            $_POST["login"],
            $_POST["password"]
        );

        //validando modelo se valido retornamos um JSON...
        $retornoJson = new JsonError("Não foi possivel cadastrar o usuário");
        if ($request->isValid()) {
            $response = $this->_userService->register($request);

            if ($response->getSuccess())
                $retornoJson = new JsonSuccess("Usuário registrado com sucesso");
            else
                $retornoJson = new JsonError($response->getMsg());
        }
        header('Content-type:application/json;charset=utf-8');
        echo json_encode($retornoJson);
    }
}
