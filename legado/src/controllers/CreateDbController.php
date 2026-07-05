<?php
    namespace controllers;
    
    class CreateDbController implements IBaseController
    {
        private $_repoSeed;

        public function __construct($factory)
        {
            $this->_repoSeed = $factory->getSeedRepository();
        }

        public function proccessRequest() : void
        {
            $this->_repoSeed->createDb();
        }
    }
?>