<?php

namespace controllers\categories;

use controllers\IBaseController;
use models\JsonSuccess;
use domain\dto\CategoryJson;

class CategoryListJsonController implements IBaseController
{
    private $_repoCategories;

    public function __construct($factory)
    {
        $this->_repoCategories = $factory->getCategoryRepository();
    }

    public function proccessRequest(): void
    {
        $categories = $this->_repoCategories->getAll();
        $categoriesJson = array();
        foreach ($categories as $cat) {
            $categoriesJson[] = new CategoryJson($cat->getCategoryId(),$cat->getTitle());
        }
        //var_dump($categories);
        header('Content-type:application/json;charset=utf-8');
        $retorno = new JsonSuccess("Categorias obtidas com sucesso");
        $retorno->categories = $categoriesJson;
        echo json_encode($retorno);
    }
}
