<?php
    namespace controllers;
    use controllers\IBaseController;
    use services\ProductService;
    use models\JsonSuccess;
    use models\JsonError;
    use domain\entities\Rating;

    class ProductRatePostController implements IBaseController
    {
        public function __construct($factory)
        {
            $this->_productRepository = $factory->getProductRepository();
        }

        public function proccessRequest(): void
        {
            $retornoJson = null;
            try {
                $rating = new Rating(
                    null,
                    filter_input(INPUT_POST, 'ProductId', FILTER_SANITIZE_STRING),
                    $_POST["Rating"],
                    $_POST["Recommended"] == "Sim" ? 1 : 0,
                    $_POST["Title"],
                    $_POST["Description"],
                    $_SESSION["userId"],
                    0
                );
                
                if (!$rating->isValid()){
                    $retornoJson = new JsonError("Ocorreram erros de validação.");
                } else {
                    $this->_productRepository->addRating($rating);
                    $retornoJson = new JsonSuccess("Sua avaliação foi computada.");
                }
                header('Content-type:application/json;charset=utf-8');
            } catch (Exception $e) {
                $retornoJson = new JsonError("Não foi possivel registrar sua avaliação.");
            }
            echo json_encode($retornoJson);
        }
    }
?>