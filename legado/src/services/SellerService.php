<?php

namespace services;

use models\User;
use models\responses\RegisterSellerResponse;
use models\SellerCreateViewModel;
use models\Address;
use models\JsonError;
use models\JsonSuccess;
use infra\Logger;
use models\SellerEditViewModel;
use models\SellerDetailsViewModel;
use Exception;

class SellerService
{
    private $_repoUser;
    private $_repoSeller;
    private $_repoState;
    private $_repoAddress;
    private $_repoProducts;

    public function __construct($factory)
    {
        $this->_repoUser = $factory->getUserRepository();
        $this->_repoSeller = $factory->getSellerRepository();
        $this->_repoState = $factory->getStateRepository();
        $this->_repoAddress = $factory->getAddressRepository();
        $this->_repoProducts = $factory->getProductRepository();
    }

    public function getDetailsViewModel()
    {
        if (!isset($_GET["id"]))
            return null;
        else {
            $seller = $this->_repoSeller->getById(intval($_GET["id"]));
            if (is_null($seller))
                return null;
            else {
                $products = $this->_repoProducts->getAllByUserIdSeller($seller->getUserId());
                return new SellerDetailsViewModel($seller, $products);
            }
        }
    }

    public function register($request)
    {
        if ($this->_repoUser->getByLogin($request->getLogin()) != null) {
            //ja existe o usuario...
            return new RegisterSellerResponse(false, "O Login informado está indisponível.");
        } else {
            try {
                //criando usuário do tipo vendedor...
                $user = new User(
                    null,
                    $request->getLogin(),
                    trim($request->getPassword()),
                    $request->getName(),
                    'vendedor',
                    "",
                    $request->getLastName()
                );
                $userId = $this->_repoUser->add($user);

                //adicionando dados de vendedor...
                $this->_repoSeller->addSimplifiedSeller($userId);
                //efetuando login
                $_SESSION["userId"] = $userId;
                $_SESSION["userName"] = stripslashes($request->getName());
                $_SESSION["role"] = stripslashes($user->getRole());

                $seller = $this->_repoSeller->getByUserId($userId);

                $_SESSION["sellerId"] = $seller->getSellerId();
                return new RegisterSellerResponse(true, "Você foi registrado com sucesso.");
            } catch (Exception $e) {
                echo 'Exceção capturada: ',  $e->getMessage(), "\n";
                //exit();
            }
        }
    }

    public function getAllPaginated()
    {
        $page = 1;
        if (isset($_GET["p"]))
            $page = intval($_GET["p"]);

        $search = null;
        if (isset($_GET["s"]))
            $search = $_GET["s"];

        $paginatedResults = $this->_repoSeller->getAll($page, $search, 5);

        return $paginatedResults;
    }

    public function add($seller, $user, $address)
    {
        //sendo pessimista e prevendo que vai dar merda...
        $retorno = new JsonError("Não foi possivel cadastrar o vendedor.");

        //validando modelo se valido retornamos um JSON.
        if ($seller->isValid() && $user->isValid()) {
            //cadastrando usuario...
            if ($this->_repoUser->getByLogin($user->getLogin()) != null)
                return new JsonError("Login indisponível.");

            $userId = $this->_repoUser->add($user);
            //echo "<br> adiciono usuario id : " . $userId . " <br>";
            if (!is_null($userId)) {
                //cadastrando endereço...
                $address->setUserId($userId);
                $addressId = $this->_repoAddress->add($address);
                $seller->setUserId($userId);
                Logger::write($userId . " - " . $addressId);
                $this->_repoSeller->add($seller);

                $retorno = new JsonSuccess("Vendedor cadastrado com sucesso.");
            }
        }
        return $retorno;
    }

    public function getCreateViewModel()
    {
        return new SellerCreateViewModel(
            $this->_repoState->getAll()
        );
    }

    public function getEditViewModel()
    {
        $sellerId = intval($_GET["id"]);
        $seller = $this->_repoSeller->getById($sellerId);
        $address = $this->_repoAddress->getFirstByUserId($seller->getUserId());
        $user = $this->_repoUser->getById($seller->getUserId());

        return new SellerEditViewModel(
            $seller,
            $this->_repoState->getAll(),
            $address,
            $user
        );
    }

    public function update()
    {
        //obtendo dados da requisição e preparando a model
        $seller = $this->_repoSeller->getById(intval($_POST["sellerId"]));
        if (is_null($seller)) {
            Logger::write("Não encontrou vendedor ao tentar editar");
            return new JsonError("Não encontrou vendedor ao tentar editar");
        } else {
            $seller->setWebsite($_POST["website"]);
            $seller->setEmail($_POST["email"]);
            if (isset($_POST["cnpj"]) && !is_null($_POST["cnpj"])) {
                Logger::write("atualizando pessoa juridica");
                $seller->setCompany($_POST["company"]);
                $seller->setCnpj($_POST["cnpj"]);
                $seller->setBranchOfActivity($_POST["branchOfActivity"]);
                $seller->setAge(null);
                $seller->setFantasyName($_POST["fantasyName"]);
                $seller->setDateOfBirth(null);
            } else {
                Logger::write("atualizando pessoa física");
                $seller->setAge($_POST["age"]);
                $seller->setDateOfBirth($_POST["dateOfBirth"]);

                $seller->setFantasyName(null);
                $seller->setCompany(null);
                $seller->setCnpj(null);
                $seller->setBranchOfActivity(null);
            }
            $updateOK =  $this->_repoSeller->update($seller);
            if ($updateOK) {
                //montando model de endereço...
                $address = $this->_repoAddress->getFirstByUserId($seller->getUserId());
                if (!is_null($address)) {
                    Logger::write("atualizando endereço do vendedor");
                    $address->setStreet($_POST["street"]);
                    $address->setCep($_POST["cep"]);
                    $address->setNeighborhood($_POST["neighborhood"]);
                    $address->setCity($_POST["city"]);
                    $address->setStateId($_POST["stateId"]);
                    $address->setComplement($_POST["complement"]);
                    $this->_repoAddress->update($address);
                } else {
                    Logger::write("adicionando endereço do vendedor");
                    $address = new Address(
                        null,
                        $seller->getUserId(),
                        $_POST["street"],
                        $_POST["cep"],
                        $_POST["neighborhood"],
                        $_POST["city"],
                        $_POST["stateId"],
                        $_POST["complement"]
                    );
                    $retornoAddress = $this->_repoAddress->add($address);
                }

                //atualizando dados de usuário...
                $user = $this->_repoUser->getById($seller->getUserId());
                $user->setName($_POST["name"]);
                $user->setLastName($_POST["lastName"]);
                $user->setCpf($_POST["cpf"]);
                $this->_repoUser->altera($user);
            }
            $retorno = new JsonSuccess("Vendedor atualizado com sucesso.");
            if ($_SESSION["role"] == "vendedor")
                $retorno->urlDestino = "/admin/produto";
            else
                $retorno->urlDestino = "/admin/vendedor";

            return $retorno;
        }
    }
}
