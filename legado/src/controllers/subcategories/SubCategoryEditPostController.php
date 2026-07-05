<?php

namespace controllers\subcategories;

use controllers\IBaseController;
use models\JsonSuccess;
use models\JsonError;
use models\SubCategory;

class SubCategoryEditPostController implements IBaseController
{
    private $_repoSubCategory;

    public function __construct($factory)
    {
        $this->_repoSubCategory = $factory->getSubCategoryRepository();
    }

    public function proccessRequest(): void
    {
        $subCategoryId = $_POST["subCategoryId"];
        $categoryId = $_POST["categoryId"];
        $title = $_POST["title"];
        $subCategory = new SubCategory($subCategoryId, $categoryId, $title);

        if ($subCategory->isValid()) {
            $this->_repoSubCategory->update($subCategory);
            $retorno = new JsonSuccess("SubCategoria alterada com sucesso");
            header('Content-type:application/json;charset=utf-8');
        } else
            $retorno = new JsonError("Não foi possivel alterar a SubCategoria");

        echo json_encode($retorno);
    }
}
