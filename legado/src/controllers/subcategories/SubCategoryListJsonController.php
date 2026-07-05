<?php

namespace controllers\subcategories;

use controllers\IBaseController;
use models\JsonSuccess;
use domain\dto\SubCategoryJson;

class SubCategoryListJsonController implements IBaseController
{
    private $_repoSubCategories;

    public function __construct($factory)
    {
        $this->_repoSubCategories = $factory->getSubCategoryRepository();
    }

    public function proccessRequest(): void
    {
        $subCategories = $this->_repoSubCategories->getAllByCategory($_GET["categoryId"]);

        $subCategoriesJson = array();
        foreach ($subCategories as $sub) {
            $subCategoriesJson[] = new SubCategoryJson($sub->getSubCategoryId(),$sub->getCategoryId(),$sub->getTitle());
        }
        header('Content-type:application/json;charset=utf-8');
        $retorno = new JsonSuccess("SubCategorias obtidas com sucesso");
        $retorno->subCategories = $subCategoriesJson;
        echo json_encode($retorno);
    }
}
