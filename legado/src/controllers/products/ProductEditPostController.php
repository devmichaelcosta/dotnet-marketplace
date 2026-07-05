<?php

namespace controllers\products;

use models\Product;
use controllers\IBaseController;
use services\ProductService;
use models\JsonError;
use models\JsonSuccess;
use domain\entities\AttributeValue;

use Exception;

class ProductEditPostController implements IBaseController
{
    private $productService;
    private $errorMsg = "Não foi possível cadastrar o produto,  ocorreram erros de validação verifique a seguir.";
    public function __construct($factory)
    {
        $this->productService = new ProductService($factory);
    }

    public function proccessRequest(): void
    {
        $retornoJson = null;        
        try {
            $product = new Product(
                $_POST["productId"],
                filter_input(INPUT_POST, 'title', FILTER_SANITIZE_STRING),
                filter_input(INPUT_POST, 'price', FILTER_SANITIZE_STRING),
                filter_input(INPUT_POST, 'description', FILTER_SANITIZE_STRING),
                null,
                'michael',
                filter_input(INPUT_POST, 'offer', FILTER_SANITIZE_STRING),
                filter_input(INPUT_POST, 'stock', FILTER_SANITIZE_STRING),
                filter_input(INPUT_POST, 'sku', FILTER_SANITIZE_STRING),
                $_SESSION["role"] === "admin" ? filter_input(INPUT_POST, 'userId', FILTER_SANITIZE_STRING) : $_SESSION["userId"],
                null
            );
            $product->setSubCategoryId(isset($_POST["subCategoryId"]) ? $_POST["subCategoryId"] : null);
            $attributesValues = json_decode($_POST['attributesValues']);
            for ($i=0; $i < count($attributesValues); $i++) 
                $product->addAttributeValue(new AttributeValue($attributesValues[$i]->attributeId,$product->getId(),$attributesValues[$i]->value));

            if (!$product->isValid())
                $retornoJson = new JsonError($this->errorMsg);
            else 
            {
                $imagesUploaded = isset($_POST['images']) ? $_POST['images'] : null;
                $this->productService->update($imagesUploaded, $product);
                $retornoJson = new JsonSuccess("Produto atualizado com sucesso.");
                header('Content-type:application/json;charset=utf-8');
            }
        } catch (Exception $e) {
            $retornoJson = new JsonError("Não foi possível cadastrar o usuário.");
        }
        echo json_encode($retornoJson);
    }
}