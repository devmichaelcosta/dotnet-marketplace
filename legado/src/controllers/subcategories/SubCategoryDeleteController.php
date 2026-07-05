<?php

namespace controllers\subcategories;

use controllers\IBaseController;
use models\JsonSuccess;
use models\JsonError;

class SubCategoryDeleteController implements IBaseController
{
    private $_repoSubCategory;

    public function __construct($factory)
    {
        $this->_repoSubCategory = $factory->getSubCategoryRepository();
    }

    public function proccessRequest(): void
    {
        try {
            $id = $_POST["id"];
            if (is_null($id) || $id === false) {
                $retorno = new JsonError("Não foi possivel encontrar a subcategoria.");
                header('Content-type:application/json;charset=utf-8');
                echo json_encode($retorno);
            } else {
                $this->_repoSubCategory->remove($_POST["id"]);
                $retorno = new JsonSuccess("SubCategoria removida com sucesso.");
                header('Content-type:application/json;charset=utf-8');
                echo json_encode($retorno);
            }
        } catch (Exception $e) {
            $retorno = new JsonError("Não foi possivel cadastrar a subcategoria.");
            header('Content-type:application/json;charset=utf-8');
            echo json_encode($retorno);
        }
    }
}
