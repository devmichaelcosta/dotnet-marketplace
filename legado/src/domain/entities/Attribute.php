<?php

namespace domain\entities;

class Attribute
{
    private $_attributeId;
    private $_name;

    private $errors;

    public function getName()
    {
        return  $this->_name;
    }

    public function getAttributeId()
    {
        return  $this->_attributeId;
    }


    public function __construct($categoryId, $name)
    {
        $this->_attributeId = $categoryId;
        $this->_name = $name;

        $this->errors = array();
    }

    public function getErrors()
    {
        return $this->errors;
    }

    public function isValid(): bool
    {
        if (is_null($this->getName()) || $this->getName() === "")
            $this->errors['name'] = 'Informe o nome.';

        return count($this->errors) === 0;
    }
}
