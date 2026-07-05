<?php

namespace controllers\categories;

use controllers\IBaseController;
use models\JsonSuccess;
use models\JsonError;


class CategoryDeleteController implements IBaseController
{
    private $_repoCategory;

    public function __construct($factory)
    {
        $this->_repoCategory = $factory->getCategoryRepository();
    }

    public function proccessRequest(): void
    {
        try {
            $id = $_POST["id"];
            if (is_null($id) || $id === false) {
                $retorno = new JsonError("Não foi possivel encontrar a categoria.");
                header('Content-type:application/json;charset=utf-8');
                echo json_encode($retorno);
            } else {
                $this->_repoCategory->remove($_POST["id"]);
                $retorno = new JsonSuccess("Categoria removida com sucesso.");
                header('Content-type:application/json;charset=utf-8');
                echo json_encode($retorno);
            }
        } catch (Exception $e) {
            $retorno = new JsonError("Não foi possivel cadastrar a categoria.");
            header('Content-type:application/json;charset=utf-8');
            echo json_encode($retorno);
        }
    }
}
