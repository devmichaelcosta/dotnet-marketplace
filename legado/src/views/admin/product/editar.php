<?php require_once './views/partials/header-admin.php' ?>
<link rel="stylesheet" href="/assets/libs/dropzone/dropzone.min.css">
<div class="container">
    <div class="d-flex align-items-center p-3 mt-3 text-white-50 bg-dark rounded shadow-sm">
        <div class="lh-100">
            <h6 class="mb-0 text-white lh-100">Cadastro de produto</h6>
            <ul class="nav-breadcrumb">
                <li>
                    <a href="/admin/produto">Produto</a>
                </li>
                <li>
                    <a href="/admin/produto/cadastrar">Cadastrar</a>
                </li>
            </ul>
        </div>
    </div>

    <div class="card mt-3">
        <div class="card-body">
            <h5>Informe os dados do produto e clique em salvar.</h6>
                <form action="/admin/produtos/editar-post" method="post" id="form-cadastro" enctype="multipart/form-data" class="">
                    <input type="hidden" id="productId" name="productId" value="<?php echo $product->getId(); ?>">

                    <input type="hidden" id="images" name="images" value="<?php echo  $product->getImagesStr(); ?>">

                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="title">Título:</label>
                                <input type="text" name="title" id="title" data-required="true" value="<?php echo $product->getTitle(); ?>" class="form-control" placeholder="" aria-describedby="helptitle">
                                <small id="helptitle" class="text-muted">Título do produto</small>
                            </div>
                        </div>

                        <?php if ($_SESSION["role"] == "admin") : ?>
                            <div class="col-md-3">
                                <div class="form-group">
                                    <label for="userId">Vendedor :</label>
                                    <select name="userId" id="userId" data-required="true" class="form-control" aria-describedby="help-userId">
                                        <option value="">Selecione o vendedor...</option>
                                        <?php foreach ($model->getSellers() as $seller) : ?>
                                            <option value="<?php echo $seller->getUserId(); ?>" <?php echo ($product->getUserId() == $seller->getUserId() ? "selected=selected" : ""); ?>>
                                                <?php echo $seller->getName(); ?>
                                            </option>
                                        <?php endforeach; ?>


                                    </select>
                                    <small id="help-userId" class="text-muted">Vendedor do produto</small>
                                </div>
                            </div>
                        <?php endif; ?>

                        <div class="col-md-3 campos-condicao" data-tipo="avista">
                            <div class="form-group">
                                <label for="price">Preço à vista:</label>
                                <input type="number" name="price" id="price" value="<?php echo $product->getPrice(); ?>" data-required="true" class="form-control" pattern="[0-9]+([\.,][0-9]+)?" step="0.01" placeholder="" aria-describedby="help-price">
                                <small id="help-price" class="text-muted">Preço à vista do produto</small>
                            </div>
                        </div>
                        <div class="col-md-3 ">
                            <div class="form-group">
                                <label for="sku">Sku:</label>
                                <input type="text" name="sku" id="sku" data-required="true" value="<?php echo $product->getSku(); ?>" class="form-control" placeholder="" aria-describedby="help-sku">
                                <small id="help-sku" class="text-muted">Código do produto</small>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-group">
                                <label for="stock">Estoque:</label>
                                <input type="number" name="stock" id="stock" data-required="true" value="<?php echo $product->getStock(); ?>" class="form-control" placeholder="" aria-describedby="help-stock">
                                <small id="help-stock" class="text-muted">Estoque do produto</small>
                            </div>
                        </div>
                        <div class="col-md-6 ">
                            <div class="form-group">
                                <label for="offer">Esse produto é oferta:</label>
                                <div class="mb-2">
                                    <input type="radio" name="offer" value="true" id="offer-sim" checked="checked" placeholder="" aria-describedby="help-stock" <?php echo $product->getOffer() == true ? "checked='checked'" : ""; ?>>
                                    <label for="offer-sim">Sim</label>
                                    <input type="radio" name="offer" value="false" id="offer-nao" <?php echo $product->getOffer() == false ? "checked='checked'" : ""; ?> placeholder="" aria-describedby="help-stock"> <label for="offer-nao">Não</label>
                                </div>
                                <small id="help-stock" class="text-muted">Se esse produto esta em oferta selecione sim</small>
                            </div>
                        </div>
                        <div class="col-md-12">
                            <div class="form-group">
                                <label for="description">Descritivo do produto:</label>
                                <textarea name="description" data-required="true" style="height:150px;" id="description" class="form-control" placeholder="" aria-describedby="help-description"><?php echo $product->getDescription(); ?></textarea>
                                <small id="help-description" class="text-muted">
                                    Descrição com as caracteristicas do produto.
                                </small>
                            </div>
                        </div>
                        
                        <div class="col-md-6 ">
                            <div class="form-group">
                                <label for="offer">Categoria:</label>
                                <div class="mb-2">
                                    <select class="form-control" id="categoryId" name="categoryId" data-category-id="<?php echo $product->getCategoryId(); ?>">
                                        <option value="">Selecione</option>
                                        <?php foreach ($model->getCategories() as $category) : ?>
                                            <option value="<?php echo $category->getCategoryId(); ?>" <?php echo $product->getCategoryId() == $category->getCategoryId() ? "selected" : "" ?>>
                                                <?php echo $category->getTitle(); ?>
                                            </option>
                                        <?php endforeach; ?>
                                    </select>
                                </div>
                                <small id="help-stock" class="text-muted"></small>
                            </div>
                        </div>
                        <div class="col-md-6 ">
                            <div class="form-group">
                                <label for="offer">Subcategoria:</label>
                                <div class="mb-2">
                                    <select class="form-control" id="subCategoryId" name="subCategoryId" disabled="disabled" data-sub-category-id="<?php echo $product->getSubCategoryId(); ?>">
                                        <option value="">Selecione </option>
                                    </select>
                                </div>
                                <small id="help-stock" class="text-muted"></small>
                            </div>
                        </div>

                        <!--imagens atuais e container de uploads-->
                        <div class="col-md-12">
                            <div class="form-group">
                                <label for="">Imagens atuais:</label>
                                <div id="product-img-cards-container" class="card-columns ">
                                    <?php if (count($product->getImages()) > 0) : ?>
                                        <?php foreach ($product->getImages() as $image) : ?>
                                            <div class="card text-center " data-name="<?php echo $image["FileName"]; ?>">
                                                <div class="text-center p-2">
                                                    <img src="/assets/img/products/<?php echo $image["FileName"]; ?>" src="..." alt="..." style="max-width:100px; max-height:100px;" class="img-fluid ">
                                                </div>
                                                <div class="card-footer p-2">
                                                    <button type="button" class="btn btn-danger btn-sm" data-name="<?php echo $image["FileName"]; ?>" 
                                                        data-product-id="<?php echo $image["ProductId"]; ?>" data-id="<?php echo $image["ProductImageId"]; ?>"
                                                        title="Remover imagem">
                                                        <i class="fa fa-trash"></i> Remover
                                                    </button>
                                                </div>
                                            </div>
                                        <?php endforeach; ?>
                                    <?php endif; ?>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12">
                            <div class="form-group">
                                <label for="">Selecione uma ou mais imagens para seu produto:</label>
                                <div id="upload-container" class="dropzone">
                                </div>
                            </div>
                        </div>

                        <div class="col-md-12">
                            <div class="mb-2">
                                <label class="pull-left">
                                    Ficha técnica:
                                </label>
                                <button type="button" class="btn btn-dark btn-sm pull-right" id="btn-add-attribute" data-product-id="<?php echo $product->getId(); ?>" data-toggle="button" aria-pressed="false" autocomplete="off">
                                    <i class="fa fa-plus"></i> Adicionar atributo
                                </button>
                                <span class="clearfix"></span>
                            </div>

                            <table class="table table-striped <?php echo (count($product->getAttributesValues()) > 0 ? "" : "hidden"); ?>" id="tb-attributes">
                                <thead>
                                    <tr>
                                        <th></th>
                                        <th>Nome</th>
                                        <th>Valor</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <?php if (count($product->getAttributesValues()) > 0) : ?>
                                        <?php foreach ($product->getAttributesValues() as $attributeValue) : ?>
                                            <tr data-attribute-id="<?php echo $attributeValue->getAttributeId(); ?>">
                                                <td width="10px" scope="row" style="vertical-align:middle">
                                                    <button type="button" class="btn btn-danger btn-sm btn-remove-attribute" data-toggle="button" aria-pressed="false" autocomplete="off">
                                                        <i class="fa fa-trash"></i>
                                                    </button>
                                                </td>
                                                <td valign="middle" style="vertical-align:middle" width="30%"><?php echo $attributeValue->getAttributeName(); ?></td>
                                                <td valign="middle" style="vertical-align:middle" width="70%">
                                                    <input type="text" value="<?php echo $attributeValue->getValue(); ?>" class="form-control" name="attribute-<?php echo $attributeValue->getAttributeId(); ?>" id="attribute-<?php echo $attributeValue->getAttributeId(); ?>" data-required="true" aria-describedby="helpId" placeholder="">
                                                    <small id="helpId" class="form-text text-muted">Valor do atributo <?php echo $attributeValue->getAttributeName(); ?></small>
                                                </td>
                                            </tr>
                                            <?php endforeach; ?>
                                    <?php endif; ?>
                                </tbody>
                            </table>

                            <div class="alert alert-info <?php echo (count($product->getAttributesValues()) == 0 ? "" : "hidden"); ?>" id="has-no-attributes">
                                Nenhum atributo foi selecionado.
                            </div>

                        </div>

                        <div class="col-md-12">
                            <a class="btn btn-sm btn-warning" href="/admin/produto">
                                <i class="fa fa-chevron-left"></i>
                            </a>
                            <button type="submit" name="btn-salvar" id="btn-salvar" data-loading-text="Processando, Aguarde..." class="btn btn-sm btn-dark">
                                <i class="fa fa-save"></i> Salvar produto
                            </button>
                        </div>
                    </div>
                </form>
        </div>
    </div>
</div>

<?php require_once './views/partials/scripts-admin.php' ?>
<script src="/assets/libs/dropzone/dropzone.min.js"></script>
<script src="/assets/js/models/GenericValidator.js"></script>
<script src="/assets/js/admin/product/Product.js"></script>
<script src="/assets/js/models/ProductDropzone.js"></script>
<script src="/assets/js/models/ProductImageCard.js"></script>
<script src="/assets/js/models/ProductCrudBase.js"></script>
<script src="/assets/js/models/ProductEdit.js"></script>
<?php require_once './views/partials/footer-admin.php' ?>