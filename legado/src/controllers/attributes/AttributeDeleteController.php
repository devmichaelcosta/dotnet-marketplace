<?php

namespace controllers\attributes;

use controllers\IBaseController;
use models\JsonSuccess;
use models\JsonError;
use Exception;

class AttributeDeleteController implements IBaseController
{
    private $_repoAttribute;

    public function __construct($factory)
    {
        $this->_repoAttribute = $factory->getAttributeRepository();
    }

    public function proccessRequest(): void
    {
        try {
            $id = $_POST["id"];
            if (is_null($id) || $id === false) {
                $retorno = new JsonError("Não foi possivel encontrar o atributo.");
                header('Content-type:application/json;charset=utf-8');
                echo json_encode($retorno);
            } else {
                $this->_repoAttribute->remove($_POST["id"]);
                $retorno = new JsonSuccess("Atributo removida com sucesso.");
                header('Content-type:application/json;charset=utf-8');
                echo json_encode($retorno);
            }
        } catch (Exception $e) {
            $retorno = new JsonError("Não foi possivel cadastrar o atributo.");
            header('Content-type:application/json;charset=utf-8');
            echo json_encode($retorno);
        }
    }
}
