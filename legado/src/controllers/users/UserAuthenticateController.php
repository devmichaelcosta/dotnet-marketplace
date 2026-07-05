<?php

namespace controllers\users;

use controllers\IBaseController;
use models\JsonSuccess;
use models\JsonError;

class UserAuthenticateController implements IBaseController
{
    private $_repoUser;
    private $_repoSeller;
    public function __construct($factory)
    {
        $this->_repoUser = $factory->getUserRepository();
        $this->_repoSeller = $factory->getSellerRepository();
    }

    public function proccessRequest(): void
    {

        $pos = strpos($_SERVER['HTTP_REFERER'], "returnto=checkout");
        $mandarParaCheckout = false;
        if ($pos > -1)
            $mandarParaCheckout = true;

        $login = filter_input(INPUT_POST, 'login', FILTER_SANITIZE_STRING);
        $password = filter_input(INPUT_POST, 'password', FILTER_SANITIZE_STRING);
        $user = $this->_repoUser->getByLogin($login);
        $loginEfetuado = false;
        if (!is_null($user)) {
            //echo 'usuario encontrado<br>';
            if ($user->passwordIsValid($_POST["password"])) {
                //echo 'senha valida<br>';
                $_SESSION["userId"] = $user->getUserId();
                
                $_SESSION["userName"] = stripslashes($user->getName());
                $_SESSION["role"] = stripslashes($user->getRole());
                $_SESSION["sellerId"] = null;

                if ($user->getRole() == "vendedor") {
                    $seller = $this->_repoSeller->getByUserId($user->getUserId());
                    $_SESSION["sellerId"] = $seller->getSellerId();
                }
                $jsonReturn = new JsonSuccess("Login realizado com sucesso.");
                $loginEfetuado = true;
                if ($mandarParaCheckout === true)
                    $jsonReturn->urlDestino = "/finalizar-pedido";
                else
                    $jsonReturn->urlDestino = "/";

                header('Content-type:application/json;charset=utf-8');
                echo json_encode($jsonReturn);
            }
        }
        if (!$loginEfetuado) {
            $jsonReturn = new JsonError("Login ou senha inválidos.");
            header('Content-type:application/json;charset=utf-8');
            echo json_encode($jsonReturn);
        }
    }
}
