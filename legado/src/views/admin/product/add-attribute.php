<div class="cabecalho">
    Adicionar atributo para ficha técnica
</div>
<div class="corpo">
    <table class="table  table-striped table-hover " id="tb-avaliable-attributes">
        <thead class="thead-dark">
            <tr>
                <th scope="col" width="20px"></th>
                <th scope="col">Nome</th>
            </tr>
        </thead>
        <tbody>
            <?php if (count($attributes) > 0) : ?>
                <?php foreach ($attributes as $attribute) : ?>
                    <tr class="cursor">
                        <td>
                            <input type="checkbox" name="cb-attribute" value="<?php echo $attribute->getAttributeId(); ?>" data-name="<?php echo $attribute->getName(); ?>" />
                        </td>
                        <td><?php echo $attribute->getName(); ?></td>
                    </tr>
                <?php endforeach; ?>
            <?php endif; ?>
        </tbody>
    </table>
</div>
<div class="rodape"></div>