<?php 
    namespace infra\interfaces;
    
    interface ISeedRepository 
    {
        public function seed();
        public function createDb(); 
        public function destroyDatabase();
    }
?>