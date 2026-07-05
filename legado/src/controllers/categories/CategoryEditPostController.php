<?php

namespace controllers\categories;

use controllers\IBaseController;
use models\JsonSuccess;
use models\JsonError;
use models\Category;

class CategoryEditPostController implements IBaseController
{
    private $_repoCategory;

    public function __construct($factory)
    {
        $this->_repoCategory = $factory->getCategoryRepository();
    }

    public function proccessRequest(): void
    {
        $categoryId = $_POST["categoryId"];
        $title = $_POST["title"];
        $category = new Category($categoryId, $title, null);

        if ($category->isValid()) {
            $imagesUploaded = null;
            if (isset($_POST['images'])) {
                $imagesUploaded = $_POST['images'];
                $category->setImage($imagesUploaded);
            }
            $this->_repoCategory->update($category);
            $retorno = new JsonSuccess("Categoria alterada com sucesso");
            header('Content-type:application/json;charset=utf-8');
        } else
            $retorno = new JsonError("Não foi possivel alterar a Categoria");

        echo json_encode($retorno);
    }
}
