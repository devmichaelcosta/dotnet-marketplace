<?php
    namespace controllers;
    use infra;
    use infra\repositories;
    use PDO;

    class DestroyDbController implements IBaseController
    {
        private $_repoSeed;

        public function __construct($factory)
        {
            $this->_repoSeed = $factory->getSeedRepository();
        }

        public function proccessRequest() : void
        {
            $this->_repoSeed->destroyDatabase();
        }
    }
?>