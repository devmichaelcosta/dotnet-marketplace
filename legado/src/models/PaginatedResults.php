<?php
    namespace models;
    
    class PaginatedResults 
    {
        public $results;
        public $qtdTotal;
        public $qtdTotalFiltered;
        public $hasPreviousPage;
        public $hasNextPage;
        public $page;
        public $numberOfPages;

        public $urlPrevPage;
        public $attrDisablePrev;
        public $urlNextPage;
        public $attrDisableNext;

        public function __construct($results, $qtdTotal, $qtdTotalFiltered,  $page,$pageSize,$urlPagina)
        {
            $this->results = $results;
            $this->qtdTotal = $qtdTotal;
            $this->qtdTotalFiltered = $qtdTotalFiltered;
            $this->page = $page;
            $this->numberOfPages = ceil($qtdTotal / $pageSize);
            $this->hasPreviousPage= ($this->numberOfPages > 1 && $page > 1) ? true : false;
            $this->hasNextPage = (intval($this->numberOfPages) > intval($page)) ? true : false;
            
            //dados de controle da paginação
            $this->urlPrevPage = $urlPagina . ($this->page - 1);
            $this->attrDisablePrev = "";
            if ($this->hasPreviousPage == false)
            $this->attrDisablePrev = "disabled";
            
            $this->urlNextPage = $urlPagina . ($this->page + 1);
            $this->attrDisableNext = "";
            if (!$this->hasNextPage)
                $this->attrDisableNext = "disabled";
            //echo 'page ' . $page . ' -- ' . $qtdTotal . ' / ' . $pageSize . '=' . $this->numberOfPages;
        }
    }
