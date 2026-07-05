<?php

namespace controllers\categories;

use controllers\IBaseController;
use models\JsonSuccess;
use models\JsonError;
use models\Category;

class CategoryCreatePostController implements IBaseController
{
    private $_repoCategory;

    public function __construct($factory)
    {
        $this->_repoCategory = $factory->getCategoryRepository();
    }

    public function proccessRequest(): void
    {
        $retorno = null;
        $category = new Category(
            null,
            filter_input(INPUT_POST, 'title', FILTER_SANITIZE_STRING),
            null
        );

        //validando modelo se valido retornamos um JSON.
        if ($category->isValid()) {
            $imagesUploaded = null;
            if (isset($_POST['images'])) {
                $imagesUploaded = $_POST['images'];
                $category->setImage($imagesUploaded);
            }
            $this->_repoCategory->add($category);
            $retorno = new JsonSuccess("Categoria cadastrada com sucesso");
            header('Content-type:application/json;charset=utf-8');
        } else
            $retorno = new JsonError("Não foi possivel cadastrar a categoria.");

        echo json_encode($retorno);
    }
}
