<?php

namespace controllers\attributes;

use controllers\IBaseController;
use models\JsonSuccess;
use models\JsonError;
use domain\entities\Attribute;

class AttributeCreatePostController implements IBaseController
{
    private $_repoAttribute;

    public function __construct($factory)
    {
        $this->_repoAttribute = $factory->getAttributeRepository();
    }

    public function proccessRequest(): void
    {
        $retorno = null;
        $attribute = new Attribute(null, filter_input(INPUT_POST, 'name', FILTER_SANITIZE_STRING));

        //validando modelo se valido retornamos um JSON.
        if ($attribute->isValid()) {
            $this->_repoAttribute->add($attribute);
            $retorno = new JsonSuccess("Atributo cadastrado com sucesso");
            header('Content-type:application/json;charset=utf-8');
        } else
            $retorno = new JsonError("Não foi possivel cadastrar o atributo.");

        echo json_encode($retorno);
    }
}
