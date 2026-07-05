<section>
    <h3 class="mt-3">Ficha técnica</h3>
    <div class="row">
        <div class="col-sm-12 col-xs-12">
            <div class="card p-3">
                <div class="row">
                    <div class="col-lg-12">
                        <table   class="100-w table table-striped ">
                            <tbody>
                                <?php foreach ($product->getAttributesValues() as $attributeValue): ?>
                                <tr>
                                    <td ><?php echo $attributeValue->getAttributeName(); ?></td>
                                    <td><?php echo $attributeValue->getValue(); ?></td>
                                </tr>
                                <?php endforeach; ?>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
</section>