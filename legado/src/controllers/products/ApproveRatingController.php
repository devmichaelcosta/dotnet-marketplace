<?php
    namespace controllers\products;
    
    use controllers\IBaseController;
    use models\JsonError;
    use models\JsonSuccess;
    use Exception;

    class ApproveRatingController implements IBaseController
    {
        private $productRepository;

        public function __construct($factory)
        {
            $this->productRepository = $factory->getProductRepository();
        }

        public function proccessRequest() : void
        {
            $retornoJson = null;
            try {
                $this->productRepository->approveRating($_POST["id"]);
                $retornoJson = new JsonSuccess("Avaliação aprovada com sucesso.");
                header('Content-type:application/json;charset=utf-8');
            } 
            catch(Exception $e)
            {
                $retornoJson = new JsonError("Não foi possivel cadastrar o produto"); 
            }
            echo json_encode($retornoJson);
        }
    }