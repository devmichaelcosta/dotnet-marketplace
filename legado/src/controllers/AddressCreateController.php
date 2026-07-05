<?php
    namespace controllers;
    use controllers\IBaseController;
    use models\Address;

    class AddressCreateController implements IBaseController
    {
        private $_repoStates;
        public function __construct($factory)
        {
            $this->_repoStates = $factory->getStateRepository();
        }
        
        public function proccessRequest() : void
        {
            $address = new Address(null,null,null,null,null,null,null,null);
            $states = $this->_repoStates->getAll();
            require "views/admin/users/address.php";
        }
    }
?>