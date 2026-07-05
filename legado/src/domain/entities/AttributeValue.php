<?php

namespace domain\entities;

class AttributeValue
{
    private $_attributeId;
    private $_value;
    private $_productId;
    private $_attributeName;

    private $errors;

    public function getValue()
    {
        return  $this->_value;
    }

    public function getAttributeId()
    {
        return  $this->_attributeId;
    }

    public function getProductId()
    {
        return  $this->_productId;
    }

    public function __construct($attributeId, $productId, $value)
    {
        $this->_attributeId = $attributeId;
        $this->_value = $value;
        $this->_productId = $productId;
        $this->errors = array();
    }

    public function setAttributeName($attributeName){
        $this->_attributeName = $attributeName;
    }
    
    public function setProductId($productId){
        $this->_productId = $productId;
    }

    public function getAttributeName(){return $this->_attributeName;}
    public function getErrors()
    {
        return $this->errors;
    }

    public function isValid(): bool 
    {
        if (is_null($this->getValue()) || $this->getValue() === "")
            $this->errors['name'] = 'Informe o valor.';

        return count($this->errors) === 0;
    }
}
