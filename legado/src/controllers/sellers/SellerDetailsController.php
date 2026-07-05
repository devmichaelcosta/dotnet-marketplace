<?php
    namespace controllers\sellers;

    use controllers\IBaseController;
    use services\SellerService;

    class SellerDetailsController implements IBaseController
    {
        private $_sellerService;

        public function __construct($factory)
        {
            $this->_sellerService = new SellerService($factory);
        }
        
        public function proccessRequest() : void
        {
            $model = $this->_sellerService->getDetailsViewModel();
            require "views/admin/sellers/detalhes.php";
        }
    }
