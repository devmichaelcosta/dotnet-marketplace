<?php

namespace services;

use models\User;
use models\responses\RegisterUserResponse;
use models\JsonSuccess;
use models\JsonError;
use models\UserEdit;
use models\Address;
use Exception;
use Infra\Logger;

class UserService
{
    private $_repoUser;
    private $_repoAddresses;
    private $_repoSeller;

    public function __construct($factory)
    {
        $this->_repoUser = $factory->getUserRepository();
        $this->_repoAddresses = $factory->getAddressRepository();
        $this->_repoSeller = $factory->getSellerRepository();
    }

    public function register($request)
    {
        if ($this->_repoUser->getByLogin($request->getLogin()) != null) //ja existe o usuario
            return new RegisterUserResponse(false, "O Login informado está indisponível.");
        else {
            try {
                //criando usuário do tipo comum...
                $user = new User(
                    null,
                    $request->getLogin(),
                    trim($request->getPassword()),
                    $request->getName(),
                    'comum',
                    '',
                    $request->getLastName()
                );

                $userId = $this->_repoUser->add($user);

                //efetuando login
                $_SESSION["userId"] = $userId;
                $_SESSION["userName"] = stripslashes($request->getName());
                $_SESSION["role"] = stripslashes($user->getRole());
                $_SESSION["sellerId"] = null;
                if ($_SESSION["role"] == "vendedor") {
                    Logger::write('registrou o vendedor');
                    $seller = $this->_repoSeller->getByUserId($userId);
                    $_SESSION["sellerId"] = $seller->getSellerId();
                }
                return new RegisterUserResponse(true, "Você foi registrado com suc0esso.");
            } catch (Exception $e) {
                return new RegisterUserResponse(
                    false,
                    "Não foi possivel registrar o usuário."
                );
            }
        }
    }

    public function update()
    {

        $role = $_SESSION["role"];

        if (isset($_POST["role"]) && $_SESSION["role"] == "admin")
            $role = $_POST["role"];

        $user = new UserEdit(
            $_POST["userId"],
            $_POST["name"],
            $_POST["login"],
            $role,
            $_POST["cpf"],
            $_POST["lastName"]
        );
        $retorno =  null;

        if ($user->isValid()) {
            $this->_repoUser->altera($user);

            //removendo endereços antigos
            $this->_repoAddresses->removeAllByUserId($user->getUserId());

            //adicionando endereços novos...
            if (isset($_POST["addresses"])) {
                foreach ($_POST["addresses"] as $address) {
                    $addressObj = new Address(
                        null,
                        $_POST["userId"],
                        $address["street"],
                        $address["cep"],
                        $address["neighborhood"],
                        $address["city"],
                        $address["stateId"],
                        $address["complement"]
                    );
                    $this->_repoAddresses->add($addressObj);
                }
            }

            $retorno = new JsonSuccess("Usuário alterado com sucesso");
            header('Content-type:application/json;charset=utf-8');
        } else
            $retorno = new JsonError("Não foi possivel cadastrar o usuário");

        return $retorno;
    }
}
