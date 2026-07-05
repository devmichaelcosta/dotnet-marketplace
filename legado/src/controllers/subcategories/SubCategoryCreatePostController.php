<?php

namespace controllers\subcategories;

use controllers\IBaseController;
use models\JsonSuccess;
use models\JsonError;
use models\SubCategory;

class SubCategoryCreatePostController implements IBaseController
{
    private $_repoCategory;

    public function __construct($factory)
    {
        $this->_repoSubCategory = $factory->getSubCategoryRepository();
    }

    public function proccessRequest(): void
    {
        $retorno = null;
        $subCategory = new SubCategory(
            null,
            filter_input(INPUT_POST, 'categoryId', FILTER_SANITIZE_STRING),
            filter_input(INPUT_POST, 'title', FILTER_SANITIZE_STRING)
        );

        if ($subCategory->isValid()) {
            $this->_repoSubCategory->add($subCategory);
            $retorno = new JsonSuccess("Sub Categoria cadastrada com sucesso");
            header('Content-type:application/json;charset=utf-8');
        } else
            $retorno = new JsonError("Não foi possivel cadastrar a sub categoria.");

        echo json_encode($retorno);
    }
}
