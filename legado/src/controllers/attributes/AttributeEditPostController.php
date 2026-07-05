<?php

namespace controllers\attributes;

use controllers\IBaseController;
use models\JsonSuccess;
use models\JsonError;
use domain\entities\Attribute;

class AttributeEditPostController implements IBaseController
{
    private $_repoAttribute;

    public function __construct($factory)
    {
        $this->_repoAttribute = $factory->getAttributeRepository();
    }

    public function proccessRequest(): void
    {
        $attributeId = $_POST["attributeId"];
        $name = $_POST["name"];
        $attribute = new Attribute($attributeId, $name);

        if ($attribute->isValid()) {
            $this->_repoAttribute->update($attribute);
            $retorno = new JsonSuccess("Atributo alterado com sucesso");
            header('Content-type:application/json;charset=utf-8');
        } else
            $retorno = new JsonError("Não foi possivel alterar atributo");

        echo json_encode($retorno);
    }
}
